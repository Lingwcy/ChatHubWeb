import * as SignalR from '@microsoft/signalr';
import { crypto } from '../Crypto/crypto';
import { ElMessage, ElNotification } from 'element-plus'
import { getMessageBox, getOfflineMessage, getGroupList, findFriendTree, getFriends } from '../common/api';
import { IGroupStore } from '../store/Istore';
import { IMsgStore } from '../models/interface/IMessageStore';
import { Friend } from './FriendsService';
export class ChatHub {
    private UserJwt!: string;
    public Options = {};
    public HubConnection!: signalR.HubConnection;
    public IsLogin: boolean = false;
    public ChatStore: any;
    public PmsgStore: IMsgStore;
    public UserInfoStore: any;
    public MsgboxStore: any;
    public AppsetStore: any;
    public GroupStore: IGroupStore;
    public FriendStore: any;

    constructor(jwt: string, ChatStore: any, PmsgStore: any, UserInfoStore: any, MsgBoxStore: any, GroupStore: IGroupStore, AppsetStore: any, FriendStore: any) {
        this.ChatStore = ChatStore;
        this.PmsgStore = PmsgStore;
        this.UserInfoStore = UserInfoStore;
        this.GroupStore = GroupStore;
        this.MsgboxStore = MsgBoxStore;
        this.FriendStore = FriendStore;
        this.AppsetStore = AppsetStore;
        this.UserJwt = jwt;
        this.Options = {
            skipNegotiation: true,
            transport: SignalR.HttpTransportType.WebSockets,
            accessTokenFactory: () => this.UserJwt,
        }
        this.HubConnection = new SignalR.HubConnectionBuilder()
            .withUrl('https://localhost:5001/MyHub', this.Options)
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
            this.GetUserOfflineMessage(this.UserInfoStore.userName);
            await this.HubConnection.invoke("SendHubKey", this.UserInfoStore.userName, crypto.gkey);
        } catch (err) {
            ElNotification({
                title: '系统错误',
                message: `连接失败,请联系管理员`,
            })
            console.log(err)
            this.IsLogin = false;
        }
    }
    private ChatMethodInitial() {
        //公共消息接收器
        this.HubConnection.on('PublicMsgReceived', (HeaderImg: string, fromUserName: string, msg: string) => {
            const payload = {
                message: msg,
                messageType: "TIMTextElem",
                messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
            }
            this.PmsgStore.messageItems[0].messageContent!.push(payload)
            this.PmsgStore.messageItems[0].messageNames.push(fromUserName)
            this.PmsgStore.messageItems[0].messageHeaders.push(HeaderImg)
            this.AppsetStore.MessageContract.OnConnectedName = 'public'
            this.AppsetStore.MessageContract.IsNewMessageCome++;
            //scrollDown()
        });
        //公共图片接收器
        this.HubConnection.on('PublicImageReceived', (HeaderImg: string, fromUserName: string, url: string) => {
            const payload = {
                message: url,
                messageType: "TIMImageElem",
                messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
            }
            this.PmsgStore.messageItems[0].messageContent!.push(payload)
            this.PmsgStore.messageItems[0].messageNames.push(fromUserName)
            this.PmsgStore.messageItems[0].messageHeaders.push(HeaderImg)
            this.AppsetStore.MessageContract.OnConnectedName = 'public'
            this.AppsetStore.MessageContract.IsNewMessageCome++;
            //scrollDown()
        });
        //群组消息接收器
        this.HubConnection.on('GroupMsgReceived', (HeaderImg: string, fromUserName: string, msg: string, groupId: string) => {
            //从GroupStore.MyGroups[]中找到groupId对应的群组信息
            let groupInfo = this.GroupStore.MyGroups.find(g => g.GroupId.toString() == groupId)
            ElNotification({
                title: `${groupInfo?.Group.GroupName}`,
                message: `${fromUserName} : ${msg}`,
            })
            //渲染到聊天区域
            let existMsgStore = false
            //如果本地有存储库 直接放
            for (let i = 0; i < this.PmsgStore.messageItems.length; i++) {
                if (this.PmsgStore.messageItems[i].targetUserName == groupInfo?.Group.GroupName) {
                    existMsgStore = true
                    const payload = {
                        message: msg,
                        messageType: "TIMTextElem",
                        messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                    }
                    this.PmsgStore.messageItems[i].messageContent!.push(payload)
                    this.PmsgStore.messageItems[i].messageNames.push(fromUserName)
                    this.PmsgStore.messageItems[i].messageHeaders.push(HeaderImg)
                    this.PmsgStore.messageItems[i].unReadCount++;
                }
            }
            if (!existMsgStore) {//如果没有这个库则创建
                this.PmsgStore.messageItems.push({
                    targetUserName: groupInfo?.Group.GroupName as string,
                    messageContent: [{
                        message: msg,
                        messageType: "TIMTextElem",
                        messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                    }],
                    messageNames: [fromUserName],
                    messageHeaders: [HeaderImg],
                    unReadCount: 1,
                })
            }
            //scrollDown()
            this.AppsetStore.MessageContract.OnConnectedName = fromUserName
            this.AppsetStore.MessageContract.IsNewMessageCome++;
        });
        //好友添加请求
        this.HubConnection.on('FriendsRequestReceived', (fromUserName) => {
            ElNotification({
                title: '系统通知',
                message: ` 来自 ${fromUserName} 的好友请求! `,
            })
        });
        //好友请求被拒绝
        this.HubConnection.on('FriendRequestRefused', (fromUserName) => {
            ElNotification({
                title: '系统通知',
                message: ` 发送给 ${fromUserName} 的好友请求被拒绝!`,
            })
        });
        //好友请求被接受
        this.HubConnection.on('FriendRequestAccepted', (fromUserName) => {
            ElNotification({
                title: '系统通知',
                message: ` 发送给 ${fromUserName} 的好友请求被接受!`,
            })
            //刷新好友列表
            this.AppsetStore.CompoentsEvent.isFriendReqestAccepted++

        });
        //离线消息接收器
        /*请求离线消息 并添加到Pinia[UseMsgbox] */
        this.HubConnection.on('MsgBoxFlasherReceived', () => {
            let payload = { username: this.UserInfoStore.userName, xusername: this.UserInfoStore.userName }
            let res = getMessageBox(payload)
            res.then(res => {
                if (res.data.code == 1) {
                    this.MsgboxStore.$reset()
                    const result = JSON.parse(res.data.data)
                    for (let i = 0; i < result.length; i++) {
                        this.MsgboxStore.MsgItems.push(result[i])
                    }
                }
            })
        });

        //即时刷新群组消息盒子
        /*请求离线消息 并添加到Pinia[UseMsgbox] */
        this.HubConnection.on('GroupMsgBoxFlasherReceived', () => {
            let payload = {
                username: this.UserInfoStore.userName,
                xusername: this.UserInfoStore.userName
            }
            getMessageBox(payload).then(result => {
                if (result.data.code == 1) {
                    this.MsgboxStore.$reset()
                    let data = JSON.parse(result.data.data)
                    for (let i = 0; i < data.length; i++) {
                        this.MsgboxStore.MsgItems.push(data[i])
                    }
                    return true
                } else if (result.data.code == 2) {
                    ElMessage({
                        message: result.data.message,
                        type: 'warning',
                    })
                    return false

                }
                return false;
            }, error => {
                ElMessage.error(error);
                return false;
            })
        });
        //私人消息接收器
        /*这里需要做到的：将消息渲染到聊天区域 */
        this.HubConnection.on('PrivateMsgReceived', (HeaderImg: string, fromUserName: string, Encryptomsg: string) => {
            let msg = crypto.decrypt(Encryptomsg);
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
            //现在考虑的前提是 接收方 也就是此客户端在线的操作（if message-receiver is online）

            let existMsgStore = false
            //如果本地有存储库 直接放
            for (let i = 0; i < this.PmsgStore.messageItems.length; i++) {
                if (this.PmsgStore.messageItems[i].targetUserName == fromUserName) {
                    existMsgStore = true
                    const payload = {
                        message: msg,
                        messageType: "TIMTextElem",
                        messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                    }
                    this.PmsgStore.messageItems[i].messageContent!.push(payload)
                    this.PmsgStore.messageItems[i].messageNames.push(fromUserName)
                    this.PmsgStore.messageItems[i].messageHeaders.push(HeaderImg)
                    this.PmsgStore.messageItems[i].unReadCount++;
                }
            }
            if (!existMsgStore) {//如果没有这个库则创建
                const payload = {
                    message: msg,
                    messageType: "TIMTextElem",
                    messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                }
                this.PmsgStore.messageItems.push({
                    targetUserName: fromUserName,
                    messageContent: [payload],
                    messageNames: [fromUserName],
                    messageHeaders: [HeaderImg],
                    unReadCount: 1,
                })
            }
            //消息发入接受端之后 在 messageBox 上渲染 且冒红点 =》 此实现位于MessageBox.vue
            this.AppsetStore.MessageContract.OnConnectedName = fromUserName
            this.AppsetStore.MessageContract.IsNewMessageCome++;
        });

        //刷新群组列表
        this.HubConnection.on('RefreshGroupList', async () => {
            const playload = {
                userId: this.UserInfoStore.userId,
                xusername: this.UserInfoStore.userName,
            }
            return await getGroupList(playload).then(res => {
                if (res.data.code == 1) {
                    this.GroupStore.MyGroups = JSON.parse(res.data.data);
                    ElNotification({
                        title: '系统通知',
                        message: `您被添加到新的群组！`,
                    })
                    return true;
                } else return false
            })
        });
        //刷新群组公告
        this.HubConnection.on('RefreshGroupNotice', async (groupName: string, notice: string) => {
            const playload = {
                userId: this.UserInfoStore.userId,
                xusername: this.UserInfoStore.userName,
            }
            return await getGroupList(playload).then(res => {
                if (res.data.code == 1) {
                    this.GroupStore.MyGroups = JSON.parse(res.data.data);
                    ElNotification({
                        title: '系统公告',
                        message: `群组${groupName}发布了新的公告`,
                    })
                    this.AppsetStore.MessageEvent.GroupNotice++;
                    return true;
                } else return false
            })
        });
        //刷新群组名称
        this.HubConnection.on('RefreshGroupName', async (groupId: number, groupName: string) => {
            //找到这个群组
            let groupInfo = this.GroupStore.MyGroups.find(g => g.GroupId == groupId)
            if (groupInfo) {
                //更新pmsg的名称
                let pmsg = this.PmsgStore.messageItems.find((m) => m.targetUserName == groupInfo!.Group.GroupName)
                console.log(groupInfo!.Group.GroupName)
                if (pmsg) {
                    console.log(pmsg)
                    pmsg.targetUserName = groupName
                }
                groupInfo.Group.GroupName = groupName
            }
            //更新msgbox上的名称
            let msgBox = this.MsgboxStore.MsgItems.find((m: any) => m.targetId == groupId)
            if (msgBox) {
                msgBox.targetfont = groupName
            }
            //更新tab上的名称 如果有的话
            let tab = this.ChatStore.targetUserTab.find((t: { tabId: number; }) => t.tabId == groupId)
            if (tab) {
                tab.tabName = groupName
                tab.tabTitle = groupName
            }
        });
        //退出群组 通知+行动
        this.HubConnection.on('DissolveGroupNotice', async (groupId: number, type: number) => {
            //type 0:被解散 1:主动退出 2:被踢出
            const taskReason = type == 0 ? "被解散" : type == 1 ? "主动退出" : "被踢出"
            //根据groupid找到这个群组
            let groupInfo = this.GroupStore.MyGroups.find(g => g.GroupId == groupId)
            if (groupInfo) {
                ElNotification({
                    title: '系统公告',
                    message: `群组${groupInfo.Group.GroupName} - ${taskReason}！`,
                })
                //从本地删除群组
                this.GroupStore.MyGroups.splice(this.GroupStore.MyGroups.indexOf(groupInfo), 1)
                this.AppsetStore.MessageEvent.GroupDissolve++;
                //如果此群组在聊天栏，则删除(this.ChatStore.targetUserTab[].tabId)
                let tab = this.ChatStore.targetUserTab.find((t: { tabId: number; }) => t.tabId == groupInfo.Group.GroupId)
                if (tab) {
                    this.ChatStore.targetUserTab.splice(this.ChatStore.targetUserTab.indexOf(tab), 1)
                }
                //删除对应的MsgBox
                let msgBox = this.MsgboxStore.MsgItems.find((m: any) => m.targetId == groupInfo.Group.GroupId)
                if (msgBox) {
                    this.MsgboxStore.MsgItems.splice(this.MsgboxStore.MsgItems.indexOf(msgBox), 1)
                }
            }
        });
        //刷新好友列表
        this.HubConnection.on('RefreshFriendList', async () => {
            const userId = this.UserInfoStore.userId
            const xusername = this.UserInfoStore.userName
            await getFriends({ userId, xusername }).then(res => {
                if (res.data.code == 1) {
                    this.FriendStore.Friends = [];
                    let data: Friend[] = JSON.parse(res.data.data);
                    let i = 0;
                    data.forEach(element => {
                        i++;
                        this.FriendStore.Friends.push(element)
                    });
                }
            })
            await findFriendTree({ userId, xusername }).then(result => {
                if (result.data.code == 1) {
                    this.FriendStore.FriendTree = JSON.parse(result.data.data)
                }
            })
        });
        //断线重连连接
        this.HubConnection.onclose(async () => {
            ElNotification({
                title: '连接断开',
            })
            await this.startHub();
        });
        this.HubConnection.onreconnected(() => {
            ElNotification({
                title: '连接重连',
            })
        });
        this.HubConnection.onreconnecting(() => {
            ElNotification({
                title: '连接中',
            })
        });
    }
    //将聊天信息挂载到此时选中的Tab ID
    public PrintMessageToTab(targetUserName: string, id: number): void {
        //从状态库中获取用户所点击的这个Tab
        this.ChatStore.targetUserTab.forEach((element: any, index: any) => {
            if (element.tabId == id) {
                this.ChatStore.selectedTab = index;
                //一但获取到这个tab。就将pmsg中的数据传递给tab进行渲染
                this.PmsgStore.messageItems.forEach((pelement: { targetUserName: string; }) => {
                    if (pelement.targetUserName == targetUserName) {
                        element.targetUserMessage = pelement
                        //引用挂起。在修改pmsg的时候同时修改chatstore(引用传递)
                    }
                });
            }
        });
    }
    //执行创建群组后的一系列即时通知任务
    public async CreateGroupTask(group: any, userIds: number[]): Promise<void> {
        await this.HubConnection.invoke("CreateGroupTask", group, userIds, this.UserInfoStore.userId);
    }
    //发送图片测试
    public async SendImgTest(imageData: any, fileType: string): Promise<void> {
        //console.log(imageData)
        await this.HubConnection.invoke("SendImage", imageData, fileType);
    }

    //发送消息
    public async SendMessageToServer(msg: string): Promise<void> {
        let selectedIndex: number = this.ChatStore.selectedTab;
        if (this.ChatStore.targetUserTab[selectedIndex].tabName == "world") {
            await this.HubConnection.invoke("SendPublicMsg", this.UserInfoStore.userName, msg);
        } else {
            let pmsg = crypto.encrypt(msg)
            if (this.ChatStore.targetUserTab[selectedIndex].tabType == 1) {
                await this.HubConnection.invoke("SendGroupMsg", this.UserInfoStore.userName, pmsg, this.GroupStore.OnConnectedGroup.GroupInfo.GroupId.toString());
                for (let i = 0; i < this.PmsgStore.messageItems.length; i++) {
                    if (this.PmsgStore.messageItems[i].targetUserName == this.ChatStore.targetUserTab[this.ChatStore.selectedTab].tabName) {
                        const payload = {
                            message: msg,
                            messageType: "TIMTextElem",
                            messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                        }
                        this.PmsgStore.messageItems[i].messageContent!.push(payload)
                        this.PmsgStore.messageItems[i].messageNames.push(this.UserInfoStore.userName)
                        this.PmsgStore.messageItems[i].messageHeaders.push(this.UserInfoStore.userImg)
                    }
                }
                //发送消息盒子提醒。刷新群组消息盒子
                await this.HubConnection.invoke("GroupMsgBoxFlasher", this.GroupStore.OnConnectedGroup.GroupInfo.GroupId.toString(), this.UserInfoStore.userName);
                return
            }
            await this.HubConnection.invoke("SendPrivateMsg", this.UserInfoStore.userName, this.ChatStore.targetUserTab[this.ChatStore.selectedTab].tabName, pmsg);
            for (let i = 0; i < this.PmsgStore.messageItems.length; i++) {
                if (this.PmsgStore.messageItems[i].targetUserName == this.ChatStore.targetUserTab[this.ChatStore.selectedTab].tabName) {
                    const payload = {
                        message: msg,
                        messageType: "TIMTextElem",
                        messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                    }
                    this.PmsgStore.messageItems[i].messageContent!.push(payload)
                    this.PmsgStore.messageItems[i].messageNames.push(this.UserInfoStore.userName)
                    this.PmsgStore.messageItems[i].messageHeaders.push(this.UserInfoStore.userImg)
                }
            }
            //发送消息盒子提醒。刷新对方消息盒子
            await this.HubConnection.invoke("MsgBoxFlasher", this.ChatStore.targetUserTab[this.ChatStore.selectedTab].tabName);
        }
    }

    public async SendImageToServer(imageUrl: string): Promise<void> {
        let selectedIndex: number = this.ChatStore.selectedTab;
        if (this.ChatStore.targetUserTab[selectedIndex].tabName == "world") {
            await this.HubConnection.invoke("SendPublicImage", this.UserInfoStore.userName, imageUrl);
        }
    }

    //从后端获取离线消息并存储到Pinia
    /*只获取不渲染*/
    public async GetUserOfflineMessage(username: string): Promise<boolean> {
        let xusername: string = username;
        return await getOfflineMessage({ username, xusername }).then(res => {
            if (res.data.code == 1) {
                let result = JSON.parse(res.data.data);
                //userInfo.unReadMsg = res.data.data.length > 0 ? false : true
                for (let x = 0; x < result.length; x++) {
                    let existMsgStore = false
                    //如果本地有存储库 直接放
                    for (let i = 0; i < this.PmsgStore.messageItems.length; i++) {
                        if (this.PmsgStore.messageItems[i].targetUserName == result[x].Sender) {
                            existMsgStore = true
                            const payload = {
                                message: result[x].SendMessage,
                                messageType: "TIMTextElem",
                                messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                            }
                            this.PmsgStore.messageItems[i].messageContent!.push(payload)
                            this.PmsgStore.messageItems[i].messageNames.push(result[x].Sender)
                            this.PmsgStore.messageItems[i].messageHeaders.push(result[x].SenderImg)
                            this.PmsgStore.messageItems[i].unReadCount = x+1;
                        }
                    }
                    if (!existMsgStore) {//如果没有这个库则创建
                        const payload = {
                            message: result[x].SendMessage,
                            messageType: "TIMTextElem",
                            messageDate: `${new Date().getHours().toString().padStart(2, '0')}:${new Date().getMinutes().toString().padStart(2, '0')}`
                        }
                        console.log(result[x].Sender)
                        this.PmsgStore.messageItems.push({
                            targetUserName: result[x].Sender,
                            messageContent: [payload],
                            messageNames: [result[x].Sender],
                            messageHeaders: [result[x].SenderImg],
                            unReadCount: x+1,
                        })
                    }
                }

                return true;
            }
            else if (res.data.code == 2) {
                ElMessage({
                    message: '获取失败',
                    type: 'warning',
                })
                return false
            }
            return false;
        }, error => {
            ElMessage.error(error);
            return false;
        })
    }
}




