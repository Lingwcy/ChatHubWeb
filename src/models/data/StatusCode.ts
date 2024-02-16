export enum ServiceStatusCode{
    Ok = 1, //业务处理成功
    Fail = 2, //业务处理失败
    NotFound = 3, //数据库中找不到数据
    RedundantAddition = 4, //冗余的添加
    AuthenticationOutDate = -3, //Token过期
}

export enum SystemStatusCode{
    NotFound = 404,
    SysyemError = 500, //内部系统错误
    UnAuthentication = 401 //没有被授权
}