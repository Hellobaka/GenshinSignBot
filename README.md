# GenshinSignBot

#### 介绍
原神米游社签到Bot

#### 使用说明
1. 安装插件
2. 私聊Bot，输入指令 #原神签到
3. 按提示完成配置
4. 每日10点进行签到
5. 如要临时重新签到, 请打开`Config.json`, 修改`LastSign`字段为昨天或昨天以前的时间(请保证时间格式), 重载即可, 插件会90秒之后进行重新签到
#### 配置说明
数据目录`data\app\me.cqp.luohuaming.GenshinSign` 配置文件`Config.json` Cookie储存文件 `Cookies.json`
![Config](https://user-images.githubusercontent.com/50934714/117919658-fb83a980-b31f-11eb-8f55-c5b85440284c.png)
- StartTime: 每日签到时间, 若要修改请保留原格式, 并且只修改时间部分, 日期部分不生效
- BroadcastGroup: 每日签到结果通知的群号, 签到之后会将结果发送到这些群 (注意: 群号直接分隔请使用英文的逗号)
- WaitSecond: 线程时间验证周期, 每隔多少秒查看时间是否到达了要签到的时间, 建议区间[20,50]
- LastSign: 上次签到时间, 自动生成, 不需要修改

#### Cookie抓取
需求：一台电脑、Chrome内核浏览器、米游社绑定原神角色

1. 打开米游社https://bbs.mihoyo.com/ys/
若已登录请退出
2. 按F12
3. 会有断点妨碍抓包，单击红框内按钮禁用断点，之后点击左侧蓝色按钮
![image.png](https://i.loli.net/2021/08/05/8NlkLvzMDeEG3gq.png)
3. 顶部切换到Network页
 ![image.png](https://i.loli.net/2021/08/05/PAvnqCEOt9BR5kW.png)
4. 此时登录，成功登录之后下一步
5. 单击这两个里面任一个
 ![image.png](https://i.loli.net/2021/08/05/HwDxJAZCPfsmaUW.png)
复制下这里的Cookie，右键，Copy value即可
5. 之后就可以将Cookie交给Bot了
