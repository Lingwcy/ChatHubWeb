declare export interface IMsgStore {
    messageItems: IMessageItems[];
}
type IMessageItems = {
    targetUserName: string,
    messageContent?: Array<{
        message: string,
        messageType: string,
        messageDate: string
    }>,
    messageNames: string[],
    messageHeaders: string[],
    unReadCount: number,
}
