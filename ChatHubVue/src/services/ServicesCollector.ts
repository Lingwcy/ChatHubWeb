import { ChatHub } from "./HubService";
import { Auth } from "./AuthService";
import { Friends } from "./FriendsService";
import { Message } from "./MessageService";
import { UserApi } from "./UserService";
import { GroupService } from "./GroupService";
import { UseChatStore, UseMsgStore, UseMsgbox, UseServiceStore, UseUserInformationStore,UseFriendsStore } from "../store";
const service = UseServiceStore();
const userInfoStore = UseUserInformationStore();
const msgStore = UseMsgStore();
const chatStore = UseChatStore();
const msgboxStore = UseMsgbox();
const friendStore = UseFriendsStore();

export function createChatHubService() {
    service.ChatHub = new ChatHub(localStorage.getItem('token') as string
        , chatStore, msgStore, userInfoStore, msgboxStore)
}
export function createAuthService() {
    service.Auth = new Auth();
}
export function createFriendsService() {
    service.Friend = new Friends();
}
export function createMessageService() {
    service.Message = new Message(msgboxStore);
}
export function createUserService(){
    service.User = new UserApi(friendStore);
}
export function createGroupService(){
    service.Group = new GroupService();
}

