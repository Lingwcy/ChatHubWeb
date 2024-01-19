import { ChatHub } from "./HubService";

export let ChatHubService:ChatHub;
export function createChatHubService(jwt:string, address:string) {
    ChatHubService = new ChatHub(jwt,address);
}
