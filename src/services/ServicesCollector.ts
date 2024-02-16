import { ChatHub } from "./HubService";
import { Auth } from "./AuthService";
import { Friends } from "./FriendsService";

export let ChatHubService: ChatHub;
export function createChatHubService(jwt: string, address: string) {
    ChatHubService = new ChatHub(jwt, address);
}

export let AuthService: Auth;
export function createAuthService() {
    AuthService = new Auth();
}

export let FriendService: Friends;
export function createFriendsService() {
    FriendService = new Friends();
}