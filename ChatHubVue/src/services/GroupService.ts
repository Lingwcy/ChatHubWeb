import { getGroupFromName } from "../common/api";

export class GroupService{
    public async SearchtGroup(groupName:string,GroupStore:any): Promise<boolean> {
        return await getGroupFromName({groupName}).then(res => {
            if(res.data.code == 1){
                GroupStore.SearchGroup = JSON.parse(res.data.data);
                return true;
            }else return false
        })
    }
}