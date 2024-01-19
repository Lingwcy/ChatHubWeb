import * as SignalR from '@microsoft/signalr';
import { ElNotification } from 'element-plus'
import axios from '../common/axiosSetting';
import { UseChatStore, UseMsgStore, UseUserInformationStore, UseMsgbox } from '../store';
const chatStore = UseChatStore();
const pmsgStore = UseMsgStore();
const userInfoStore = UseUserInformationStore();
const msgBoxStore = UseMsgbox();
export class ChatHub {
    private UserJwt!: string;
    private ServerHubAddress!: string;
    public Options = {};
    public HubConnection!: signalR.HubConnection;
    public IsLogin: boolean = false;

    constructor(jwt: string, address: string) {
        this.UserJwt = jwt;
        this.ServerHubAddress = address;
        this.Options = {
            skipNegotiation: true,
            transport: SignalR.HttpTransportType.WebSockets,
            accessTokenFactory: () => this.UserJwt,
        }
        this.HubConnection = new SignalR.HubConnectionBuilder()
            .withUrl(this.ServerHubAddress, this.Options)
            .build();
        // 断线重连
        this.HubConnection.onclose(async () => {
            ElNotification({
                title: '连接断开',
            })
            if (this.IsLogin) {
                await this.startHub();
            } else {
                this.HubConnection.stop();//窗口关闭断开通信
            }
        });
    }
    public async startHub(): Promise<void> {
        if (this.IsLogin) { return }
        try {
            await this.HubConnection.start();
            ElNotification({
                title: '欢迎',
                message: `成功连接到ChatHub`,
            })
            this.IsLogin = true;
            this.ChatMethodInitial();
            this.GetUserMessageFromServer();
        } catch (err) {
            ElNotification({
                title: '系统错误',
                message: `连接失败,请联系管理员`,
            })
            this.IsLogin = false;
        }
    }
    private ChatMethodInitial() {
        //公共消息接收器
        this.HubConnection.on('publicMsgReceived', (HeaderImg: string, fromUserName: string, msg: string) => {
            pmsgStore.messageItems[0].messages.push(msg)
            pmsgStore.messageItems[0].messageNames.push(fromUserName)
            pmsgStore.messageItems[0].messageHeaders.push(HeaderImg)
            //scrollDown()
        });
        //好友添加请求
        this.HubConnection.on('FriendsRequestReceived', (fromUserName) => {
            ElNotification({
                title: '新的好友请求!',
                message: ` 来自 :${fromUserName} `,
            })
        });
        //离线消息接收器(漏洞-请求权限问题)
        /*请求离线消息 并添加到Pinia[UseMsgbox] */
        this.HubConnection.on('MsgBoxFlasherReceived', (_fromUserName: string) => {
            axios.get('api/friends/msg-box-list/' + userInfoStore.userName + '')
                .then(res => {
                    msgBoxStore.$reset()
                    let msgitems: string[] = res.data.data.split('$')
                    for (let i = 0; i < msgitems.length - 1; i++) {
                        msgBoxStore.MsgItems.push(JSON.parse(msgitems[i]) as never)
                    }
                })
                .catch(errors => {
                    alert(errors)
                })
        });
        //私人消息接收器
        /*这里需要做到的：将消息渲染到聊天区域 */
        this.HubConnection.on('PrivateMsgReceived', (HeaderImg: string, fromUserName: string, msg: string) => {
            ElNotification({
                title: '新的消息',
                message: `${fromUserName} : ${msg}`,
            })
            /*
                私聊消息接收区  
                此方法需要做到的操作: 
                   1. 接受fromUserName(发送者) 和 msg(讯息体) 并存储到相应的 消息状态库(key=>fromUserName) 如果没有则创建
                   2. 判断接受者（本客户端）是否处于和发送者Tab打开状态(换言之 判断状态库中 editableTabs 是否存在 key=>fromUserName)
                        2.1 如果不存在此Tab => 把消息记录到中央服务器的临时表中 并在 消息盒子栏浮出红点(实现方法=>登陆时查询消息临时表 有记录则冒红点)
                        2.2 临时消息记录将渲染在 消息盒子中
                        2.3 当用户点击消息盒子信息 => 将临时消息表内容分发到 消息状态库中 且 渲染消息状态库到dom上 同时删除临时消息表中的对应元组 
                    
                
            */
            //现在考虑的前提是 接收方 也就是此客户端在线下的操作（if message-receiver is online）

            let existMsgStore = false
            //如果本地有存储库 直接放
            for (let i = 0; i < pmsgStore.messageItems.length; i++) {
                if (pmsgStore.messageItems[i].targetUserName == fromUserName) {
                    existMsgStore = true
                    pmsgStore.messageItems[i].messages.push(msg)
                    pmsgStore.messageItems[i].messageNames.push(fromUserName)
                    pmsgStore.messageItems[i].messageHeaders.push(HeaderImg)
                }
            }
            if (!existMsgStore) {//如果没有这个库则创建
                pmsgStore.messageItems.push({
                    targetUserName: fromUserName,
                    messages: [msg],
                    messageNames: [fromUserName],
                    messageHeaders: [HeaderImg],
                })
            }
            //消息发入接受端之后 在 messageBox 上渲染 且冒红点 =》 此实现位于MessageBox.vue
        });
    }
    //将聊天信息挂载到此时选中的Tab
    public PrintMessageToTab(targetUserName: string): void {
        //从状态库中获取用户所点击的这个Tab
        chatStore.targetUserTab.forEach((element, index) => {
            if (element.tabName == targetUserName) {
                chatStore.selectedTab = index;
                //一但获取到这个tab。就将pmsg中的数据传递给tab进行渲染
                pmsgStore.messageItems.forEach(pelement => {
                    if (pelement.targetUserName == targetUserName) {
                        element.targetUserMessage = pelement
                        //引用挂起。在修改pmsg的时候同时修改chatstore(引用传递)
                    }
                });
            }
        });
    }
    //发送消息
    public async SendMessageToServer(msg: string):Promise<void>{
        let selectedIndex: number = chatStore.selectedTab;
        if (chatStore.targetUserTab[selectedIndex].tabName == "world") {
            await this.HubConnection.invoke("SendPublicMsg", userInfoStore.userName, msg);
        } else {
            await this.HubConnection.invoke("SendPrivateMsg", chatStore.targetUserTab[chatStore.selectedTab].tabName, msg);
            for (let i = 0; i < pmsgStore.messageItems.length; i++) {
                if (pmsgStore.messageItems[i].targetUserName == chatStore.targetUserTab[chatStore.selectedTab].tabName) {
                    pmsgStore.messageItems[i].messages.push(msg)
                    pmsgStore.messageItems[i].messageNames.push(userInfoStore.userName)
                    pmsgStore.messageItems[i].messageHeaders.push(userInfoStore.userImg)
                }
            }
            //发送消息盒子提醒。刷新对方消息盒子
            await this.HubConnection.invoke("MsgBoxFlasher", chatStore.targetUserTab[chatStore.selectedTab].tabName);
        }
    }

    //从后端获取离线消息并存储到Pinia
    /*只获取不渲染*/
    private GetUserMessageFromServer = () => {
        axios.get('https://localhost:5001/api/message/offline-message/' + userInfoStore.userName)
            .then(res => {
                let result = res.data.data
                //userInfo.unReadMsg = res.data.data.length > 0 ? false : true
                for (let x = 0; x < result.length; x++) {
                    let existMsgStore = false
                    //如果本地有存储库 直接放
                    for (let i = 0; i < pmsgStore.messageItems.length; i++) {
                        if (pmsgStore.messageItems[i].targetUserName == result[x].sender) {
                            existMsgStore = true
                            pmsgStore.messageItems[i].messages.push(result[x].sendMessage)
                            pmsgStore.messageItems[i].messageNames.push(result[x].sender)
                            pmsgStore.messageItems[i].messageHeaders.push(result[x].senderImg)
                        }
                    }
                    if (!existMsgStore) {//如果没有这个库则创建
                        pmsgStore.messageItems.push({
                            targetUserName: result[x].sender,
                            messages: [result[x].sendMessage],
                            messageNames: [result[x].sender],
                            messageHeaders: [result[x].senderImg],
                        })
                    }
                }
            })
            .catch(err => {
                alert(err)
            })
    }
}



