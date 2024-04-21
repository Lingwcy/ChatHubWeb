import { ElMessage } from "element-plus";
import { getGroupFromName, postCreateGroup,getGroupList,getGroupMembers,postGroupRequest,getGroupRequest,postAcceptGroupRequest,rejectRequest,acceptRequest, deleteRejectGroupRequest} from "../common/api";
import { IGroupStore } from "../store/Istore";
interface SendGroupRequestParams {
    ReqMsg: string,
    UserId: Number,
    GroupId: Number,
    xusername:string
}
interface GroupRequestList {
    Id:string,
    ReqMsg:string,
    TargetGroupId:number,
    UserId:number,
    GroupName:string,
}
export interface CreateGroupModel{
    Name:string,
    UserId:number[],
    CreateUserId:number,
    xusername:string
}
interface GroupRequestParams {
    Id: number,
    xusername: string,
}
export class GroupService{
    public async SearchtGroup(groupName:string,GroupStore:any): Promise<boolean> {
        return await getGroupFromName({groupName}).then(res => {
            if(res.data.code == 1){
                GroupStore.SearchGroup = JSON.parse(res.data.data);
                return true;
            }else return false
        })
    }
    public async GetGroupList(id:number,name:string,GroupStore:any): Promise<boolean> {
        const playload = {
            userId:id,
            xusername:name
        }
        return await getGroupList(playload).then(res => {
            if(res.data.code == 1){
                GroupStore.MyGroups = JSON.parse(res.data.data);
                return true;
            }else return false
        })
    }
    public async GetGroupMemberList(id:number,name:string,GroupStore:any): Promise<boolean> {
        const playload = {
            groupId:id,
            xusername:name
        }
        return await getGroupMembers(playload).then(res => {
            if(res.data.code == 1){
                GroupStore.OnConnectedGroup.GroupMemebers= JSON.parse(res.data.data);
                return true;
            }else return false
        }) 
    }

    public async GetGroupRequestList(id:number,name:string,GroupStore:IGroupStore): Promise<boolean> {
        const playload = {
            userId:id,
            xusername:name
        }
        return await getGroupRequest(playload).then(res => {
            if(res.data.code == 1){
                let data:GroupRequestList[] = JSON.parse(res.data.data);
                //遍历 GroupStore.MyGroups。如果存在一个元素的Group.GroupId与data的TargetGroupId相等，则为data增加一个属性名为GroupName.值为遍历元素的Group.GroupName。
                data.forEach(item => {
                    GroupStore.MyGroups.forEach(group => {
                        if(group.GroupId == item.TargetGroupId){
                            item.GroupName = group.Group.GroupName;
                        }
                    })
                })
                GroupStore.GroupRequestList = data;
                return true;
            }else return false
        }) 
    }


    public async SendGroupRequest(params: SendGroupRequestParams): Promise<boolean> {
        return await postGroupRequest(params).then(result => {
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: result.data.message,
                })
                console.log(JSON.parse(result.data.data));
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

    
    public async RejectGroupRequest(params: GroupRequestParams): Promise<boolean> {
        return await deleteRejectGroupRequest(params).then(result => {
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

    public async AcceptGroupRequest(params: GroupRequestParams): Promise<boolean> {
        return await postAcceptGroupRequest(params).then(result => {
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: '添加成功',
                })
                return true;
            }
            else if (result.data.code == 2) {
                ElMessage({
                    message: '添加失败',
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

    public async CreateGroup(params: CreateGroupModel): Promise<[boolean,any]> {
        return await postCreateGroup(params).then(result => {
            if (result.data.code == 1) {
                ElMessage({
                    type: 'success',
                    message: result.data.message,
                })
                return [true,JSON.parse(result.data.data)];
            } else if (result.data.code == 2) {
                ElMessage({
                    type: 'error',
                    message: result.data.message,
                  })
                  return [false,null];

            } else if (result.data.code == 3) {
                ElMessage({
                    type: 'error',
                    message: result.data.message,
                  })
                  return  [false,null];
            }
            return  [false,null];
        }, error => {
            ElMessage.error(error);
            return  [false,null];
        })
    }



}