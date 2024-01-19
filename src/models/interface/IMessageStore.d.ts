declare export interface IMsgStore {
    messageItems: ImessageItems[];
}
type ImessageItems = {
    targetUserName: string,
    messages: string[],
    messageNames: string[],
    messageHeaders: string[]
}