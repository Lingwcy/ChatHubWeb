#!/bin/bash  
  
# 启动ssh-agent  
eval 'ssh-agent -s'  
  
# 添加私钥到ssh-agent  
ssh-add ~/.ssh/test  
  
# 生成唯一的提交消息  
commit_message=$(date +'%Y-%m-%d %H:%M:%S')  
  
# 添加和提交文件  
git add .  
git commit -m "$commit_message"  
  
# 推送  
git push -u ChatClient main