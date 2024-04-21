import { defineStore, createPinia } from 'pinia'
import { IChatStore, IGroupStore, IService, IUseFriendsStore} from './Istore';
import piniaPluginPersist from 'pinia-plugin-persist';
import { IMsgStore } from '../models/interface/IMessageStore';

export const UseUserInformationStore = defineStore("userInfo", {
    state: () => {
        return {
            userId: '',
            userName: "未登录",
            userPsw: "",
            jwtToken: "",
            userImg: "",
            connection: null,
            isLogin: false,
            unReadMsg: "new",
            UserDetailInfo: {
                id: '',
                Password: '',
                Username: '',
                HeaderImg: '',
                Email: '',
                City: '',
                Sex: '',
                Age: '',
                Job: '',
                Phone: '',
                NickName: '',
                Birth: '',
                Desc: '',
                status: ''
            }
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: 'userinfo',
                storage: localStorage,
            }
        ]
    },
    actions: {
        logout() {
            this.isLogin = false
            this.userId = ''
            this.userName = "未登录"
            this.userPsw = ""
            this.jwtToken = ""
            this.userImg = ""
            this.connection = null
            this.UserDetailInfo = {
                id: '',
                Password: '',
                Username: '',
                HeaderImg: '',
                Email: '',
                City: '',
                Sex: '',
                Age: '',
                Job: '',
                Phone: '',
                NickName: '',
                Birth: '',
                Desc: '',
                status: ''
            }
        },
    },


})
export const UseFriendsStore = defineStore("friendstore", {
    state: (): IUseFriendsStore => {
        return {
            FriendTree: {
                OwnerId:0,
                Units:[]
            },
            Friends: [],
            RequestList: [],
            TargetUserProfile: undefined //在查看好友详细资料时挂载
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: 'friends',
                storage: localStorage,
            }
        ]
    }
})

//貌似将公共消息与私人消息都存储到了这里...
export const UseMsgStore = defineStore('msg', {
    state: (): IMsgStore => {
        return {
            //公共消息name一律为world,私人消息name则为目标用户名称
            messageItems: [
                {
                    targetUserName: 'world',
                    messages: [],
                    messageNames: [],
                    messageHeaders: []
                }
            ]
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: '消息存储库',
                storage: localStorage,
            }
        ]
    }
})

export const appsetting = defineStore('appset', {
    state: () => {
        return {
            input: "",
            ServerAddress: "https://localhost:5001",
            ServerHubAddress: "https://localhost:5001/MyHub",
            NavBarShow: true,
            MessageContract:{
                OnConnectedName:"0",
                IsNewMessageCome:0,
            },
            CompoentsEvent: {
                isAddOpen: false,
                isAddDetail:{
                    isOpen:false,
                    mode:"user"//user OR group
                },
                isAcceptDetail:{
                    selectedUser:{
                        UserImg: "",
                        UserId: 0,
                        UserName: "",
                        TargetId: 0,
                        TargetName: "",
                        TargetImg: "",
                        remark: "",
                        ReqMsg: "",
                        TargetGroupId:0,
                        AccepterGroupId:0,
                        xusername:"" ,
                    },
                    isOpen:false
                },
                isFriendRequestMessageOpen:false,
                isGroupRequestMessageOpen:false,
                isFriendManagerSystemOpen:false,
                isGroupCreateOpen:false,
                isSlideVerify:{
                    isOpen:false,
                    isSuccess:false,
                },
                isContractSwitch:{
                    isFriendOpen:true,
                    isGroupOpen:false,
                },
            }
        }
    },
})

//消息栏仓库
export const UseMsgbox = defineStore('MsgBox', {
    state: () => {
        return {
            MsgCount: 0,
            MsgItems: []

        }
    },
})
//主题仓库
export const UseTheme = defineStore('Theme', {
    state: () => {
        return {
            Icon: {
                Maxmize: "/src/images/icon/dark/放大.svg",
                SwitchTheme: {
                    isDark: false,
                    url: "sss"
                }
            }

        }
    },
})
export const UseChatStore = defineStore('Chat', {
    state: (): IChatStore => {
        return {
            targetUserTab: [{
                tabTitle: "世界频道",
                tabName: "world",
                targetUserMessage: {
                    targetUserName: '',
                    messages: [],
                    messageNames: [],
                    messageHeaders: []
                },
                tabType:0,
                tabId:0,
            }],
            selectedTab: 0
        }
    },
})
export const UseGroupStore = defineStore('Group', {
    state: (): IGroupStore => {
        return {
            SearchGroup: [
                
            ],
            SearchUser:[],
            SelectedGroup:{
                GroupId: 0,
                GroupName: '',
                GroupDescription: '',
                CreationDate: '',
                CreatorUserId: 0,
                IsDeleted: false,
                MemberNumber: 0,
                GroupHeader: ''
            },
            SelectedUser:{
                id: 0,
                Username: '',
                HeaderImg: '',
                Email: '',
                City: '',
                Sex: '',
                Age: '',
                Job: '',
                Phone: '',
                NickName: '',
                Birth: '',
                Desc: '',
                status: 0
            },
            MyGroups:[],
            //正在对话中的群组信息
            OnConnectedGroup:{
                GroupInfo:{
                    GroupId: 0,
                    GroupName: '',
                    GroupDescription: '',
                    CreationDate: '',
                    CreatorUserId: 0,
                    IsDeleted: false,
                    MemberNumber: 0,
                    GroupHeader: ''
                },
                GroupMemebers:[]
            },
            GroupRequestList:[]
        }
    },
})

export const UseServiceStore = defineStore('service', {
    state: (): IService => {
        return {
            Auth: undefined,
            ChatHub: undefined,
            Friend: undefined,
            Message: undefined,
            User: undefined,
            Group: undefined,
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: '服务数据',
                storage: localStorage,
            }
        ]
    }

})
const store = createPinia()
store.use(piniaPluginPersist)
export default store

