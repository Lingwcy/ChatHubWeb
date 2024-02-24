import { ElMessage } from "element-plus";
import { getFriendRequest, getFriends, postFriendRequest, findFriend,acceptRequest, rejectRequest } from "../common/api";
import { IUseFriendsStore } from "../store/Istore";

export interface UserRequest {
    UserImg: string;
    UserId: number;
    UserName: string;
    TargetId: number;
    TargetName: string;
    TargetImg: string;
    remark: string;
    ReqMsg: string;
    xusername:string
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
    mark: string,
    ReqMsg: string,
    userName: string,
    targetName: string,
    xusername:string
}
interface FindFriendParams {
    targetName: string,
    userName: string,
    xusername:string
}
export class Friends {
    public async GetUserFriendsRquest(userName: string, friendsStore: IUseFriendsStore): Promise<boolean> {
        let xusername:string = userName
        return await getFriendRequest({ userName,xusername }).then(result => {
            if (result.data.code == 1) {
                friendsStore.RequestList = [];
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

    public async GetUserFriends(userId: string,username:string, friendsStore: IUseFriendsStore): Promise<boolean> {
        let xusername:string = username
        return await getFriends({ userId,xusername }).then(result => {
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
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: result.data.message,
                })
                // await userInfo.connection.invoke("SendFriendsRequest", TagetName);
                return true;
            } else if (result.data.code == 2) {
                ElMessage({
                    type: 'error',
                    message: result.data.message,
                  })
                  return false;

            } else if (result.data.code == 3) {
                ElMessage({
                    type: 'error',
                    message: result.data.message,
                  })
                  return false;
            }
            return false;
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
                return true;
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

    public async AcceptFriendRequest(params: UserRequest): Promise<boolean> {
        return await acceptRequest(params).then(result => {
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: result.data.message,
                })
                return true;
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

    public async RejectFriendRequest(params: UserRequest): Promise<boolean> {
        return await rejectRequest(params).then(result => {
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: '删除成功',
                })
                return true;
            }
            else if (result.data.code == 2) {
                ElMessage({
                    message: '删除失败',
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