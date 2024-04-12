declare interface ILoginFormModel {
    account: string,
    password: string,
    jwtToken: string,
}
declare interface IRegisterFormModel {
    account: string,
    password: string,
    repassword: string,
    jwtToken: string,
}


export {ILoginFormModel,IRegisterFormModel }