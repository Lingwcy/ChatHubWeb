import { ElMessage } from "element-plus";
import { getUserInfo } from "../common/api";

export class UserApi{
    public FriendStore:any
    constructor(friendStore:any){
        this.FriendStore = friendStore;
    }
    public async GetUserInfo(targetname:string): Promise<boolean> {
        return await getUserInfo({targetname}).then(
            result => {
                if(result.data.code == 1){
                    this.FriendStore.TargetUserProfile = JSON.parse(result.data.data)
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