import { ElMessage } from "element-plus";
import { getFriendRequest, getFriends, postFriendRequest, findFriend } from "../common/api";
import { IUseFriendsStore } from "../store/Istore";
import { stringifyQuery } from "vue-router";

export interface UserRequest {
    UserImg: string;
    UserId: number;
    UserName: string;
    TargetId: number;
    TargetName: string;
    TargetImg: string;
    remark: string;
    ReqMsg: string;
}
export interface Friend {
    Id: number,
    TheUserId: number,
    FriendId: number,
    FriendName: string,
    FriendImg: string,
    remark: string
}
interface SendRequestParams {
    targetName: string,
    userName: string,
    ReqMsg: string,
    mark: string
}
interface FindFriendParams {
    targetName: string,
    userName: string,
}

export class Friends {
    public async GetUserFriendsRquest(userName: string, friendsStore: IUseFriendsStore): Promise<boolean> {
        return await getFriendRequest({ userName }).then(result => {
            if (result.data.code == 1) {
                let data: UserRequest[] = JSON.parse(result.data.data);
                data.forEach(element => {
                    friendsStore.RequestList.push(element)
                });
                return true
            } else if (result.data.code == 2) {
                ElMessage({
                    message: result.data.message,
                    type: 'warning',
                })
                return false
            }
            return false
        }, error => {
            ElMessage.error(error);
            return false;
        })
    }

    public async GetUserFriends(userId: string, friendsStore: IUseFriendsStore): Promise<boolean> {
        return await getFriends({ userId }).then(result => {
            if (result.data.code == 1) {
                friendsStore.$reset()
                let data: Friend[] = JSON.parse(result.data.data);
                let i = 0;
                data.forEach(element => {
                    i++;
                    friendsStore.Friends.push(element)
                });
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

    public async SendFriendRequest(params: SendRequestParams): Promise<boolean> {
        return await postFriendRequest(params).then(result => {
            return true;
        }, error => {
            ElMessage.error(error);
            return false;
        })
    }

    public async FindFriend(params: FindFriendParams): Promise<boolean> {
        return await findFriend(params).then(result => {
            if (result.data.code == 1) { 
                ElMessage({
                    type: 'success',
                    message: `找到用户: ${params.targetName}`,
                  })
            } 
            else if (result.data.code == 2) {
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
}