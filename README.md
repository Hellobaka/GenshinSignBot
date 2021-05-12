# GenshinSignBot

#### 介绍
原神米游社签到Bot

#### 使用说明
1. 安装插件
2. 私聊Bot，输入指令 #原神签到
3. 按提示完成配置
4. 每日10点进行签到

#### 配置说明
数据目录`data\app\me.cqp.luohuaming.GenshinSign` 配置文件`Config.json` Cookie储存文件 `Cookies.json`
![image](Config)
- StartTime: 每日签到时间, 若要修改请保留原格式, 并且只修改时间部分, 日期部分不生效
- BroadcastGroup: 每日签到结果通知的群号, 签到之后会将结果发送到这些群 (注意: 群号直接分隔请使用英文的逗号)
- WaitSecond: 线程时间验证周期, 每隔多少秒查看时间是否到达了要签到的时间, 建议区间[20,60]
- LastSign: 上次签到时间, 自动生成, 不需要修改
