import { defineStore, createPinia } from 'pinia'
import { IChatStore, IService, IUseFriendsStore } from './Istore';
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
    },


})
export const UseFriendsStore = defineStore("friendstore", {
    state: (): IUseFriendsStore => {
        return {
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
            NavBarShow:true
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
                }
            }],
            selectedTab: 0
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
            User: undefined
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

