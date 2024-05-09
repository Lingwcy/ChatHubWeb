
export function ChangeTheme(ThemeStore: any): void {
    var a: HTMLAnchorElement = document.getElementById("themeSwitch") as HTMLAnchorElement;
    if (ThemeStore.Icon.SwitchTheme.isDark) {
        a.href = "src/theme/dark.css";
        document.body.style.backgroundImage = "url(./src//images/bg2.jpg)";
        ThemeStore.Icon.Maxmize = WhiteTheme.Icon.Maxmize;
    } else {
        a.href = "src/theme/default.css";
        document.body.style.backgroundImage = "url(./src//images/bg.jpg)";
        ThemeStore.Icon.Maxmize = DarkTheme.Icon.Maxmize;
    }
}


let DarkTheme = {
    Icon:{
        Maxmize:"/src/images/icon/dark/放大.svg",
        SwitchTheme:{
            isDark:true,
            url:"sss"
        }
    }
}

let WhiteTheme = {
    Icon:{
        Maxmize:"/src/images/icon/white/放大.svg",
        SwitchTheme:{
            isDark:true,
            url:"sss"
        }
    }
}