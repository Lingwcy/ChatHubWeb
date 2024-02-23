import { ElMessage } from "element-plus";
import { getMessageBox,postRedbob,deleteMessageBox } from "../common/api";
interface RedbobParmas{
    username:string,
    targetname:string
}
export class Message {
    private MessageBoxStore:any;
    constructor(msgBoxStore:any){
        this.MessageBoxStore = msgBoxStore;
    }
    public async GetMessageBox(username: string): Promise<boolean> {
        return await getMessageBox({ username }).then(result => {
            if (result.data.code == 1) {
                this.MessageBoxStore.$reset()
                let data = JSON.parse(result.data.data)
                for (let i = 0; i < data.length; i++) {
                    this.MessageBoxStore.MsgItems.push(data[i])
                }
                return true
            } else if (result.data.code == 2) {
                ElMessage({
                    message: result.data.message,
                    type: 'warning',
                })
                return false

            }
            return false;
        }, error => {
            ElMessage.error(error);
            return false;
        })
    }
    public async PostRedbob(params: RedbobParmas): Promise<boolean> {
        return await postRedbob(params).then(
            result => {
                if(result.data.code == 1){
                    return true;
                }else if(result.data.code == 2){
                    ElMessage({
                        message: result.data.message,
                        type: 'warning',
                    })
                    return false
                }
                return false
            },
            error => {
                ElMessage.error(error);
                console.log(error)
                return false;
            }
        );
    }
    public async DeleteMessageBoxItem(id:number): Promise<boolean> {
        return await deleteMessageBox({id}).then(
            result => {
                if(result.data.code == 1){
                    this.MessageBoxStore.MsgItems.forEach((value:any, index:any, array:any) => {
                        if (value.id == id) {
                            array.splice(index, 1)
                        }
                    })
                    return true;
                }else if(result.data.code == 2){
                    ElMessage({
                        message: result.data.message,
                        type: 'warning',
                    })
                    return false
                }
                return false
            },
            error => {
                ElMessage.error(error);
                console.log(error)
                return false;
            }
        );
    }

}