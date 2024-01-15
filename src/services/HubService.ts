import * as SignalR from '@microsoft/signalr';
import { ElNotification } from 'element-plus'

export class ChatHub {
    private UserJwt!: string;
    private ServerHubAddress!: string;
    public Options = {};
    public HubConnection!: signalR.HubConnection;
    public IsLogin: boolean = false;

    constructor(jwt: string, address: string) {
        this.UserJwt = jwt;
        this.ServerHubAddress = address;
        this.Options = {
            skipNegotiation: true,
            transport: SignalR.HttpTransportType.WebSockets,
            accessTokenFactory: () => this.UserJwt,
        }
        this.HubConnection = new SignalR.HubConnectionBuilder()
            .withUrl(this.ServerHubAddress, this.Options)
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
        } catch (err) {
            ElNotification({
                title: '系统错误',
                message: `连接失败,请联系管理员`,
            })
            this.IsLogin = false;
        }
    }
}



