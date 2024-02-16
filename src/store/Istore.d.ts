import { Friend,UserRequest } from '../services/FriendsService';

export interface IUseFriendsStore{
    $reset(): unknown;
    Friends: Friend[],
    RequestList: UserRequest[]
}

declare export interface IChatStore {
    targetUserTab: TargetUserTab[]
    selectedTab: number
}
//存储与用户正在聊天的Tab信息
interface TargetUserTab {
    tabTitle: string;
    tabName: string;
    targetUserMessage: TargetUserMessage;
}
//存储目标聊天用户的相关信息。名称，聊天内容...
interface TargetUserMessage {
    targetUserName: string,
    messages: string[],
    messageNames: string[],
    messageHeaders: string[],
}