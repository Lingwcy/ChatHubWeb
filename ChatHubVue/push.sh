#!/usr/bin/expect -f  
  
# 启动ssh-agent  
spawn eval `ssh-agent -s` 
expect "SSH_AUTH_SOCK="
  
# 添加私钥到ssh-agent，自动输入密码  
spawn ssh-add ~/.ssh/test  
expect "Enter passphrase"  
send "111111\r"  
expect "$ "  
  
# 添加和提交文件  
git add .  
git commit -m "$(date +'%Y-%m-%d %H:%M:%S')"  
git push -u ChatClient main