declare export interface IMsgStore {
    messageItems: ImessageItems[];
}
type ImessageItems = {
    targetUserName: string,
    messageContent: [{
        message:string
        messageType:string,
        messageDate: string
    }],
    messageNames: string[],
    messageHeaders: string[],
    unReadCount: number,
}