import { getGroupFromName } from "../common/api";

export class GroupService{
    public async SearchtGroup(groupName:string): Promise<boolean> {
        return await getGroupFromName({groupName}).then(res => {
            console.log(res.data)
            return true;
        })
    }
}