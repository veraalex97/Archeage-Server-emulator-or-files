Language: Chinese


#上古世纪服务端模拟器（基于北美服（中国区服务器可能同样适用））4.0版本
archeRage 2.9
archeRage 3.0.0.7
archeage 4.0+

这个项目被放弃了 请前往[NL0bP](https://github.com/NL0bP/Archeage-Server-emulator)

没有什么意外的话，这个项目将只作为个人测试使用


请阅读：
请不要将该项目用于商业或其他用途
原则上也不得用于个人研究等行为
否则你可能需要遵守当地的法律

Server simulator in the last century (based on the North American clothing (Chinese area server may also apply))


说明：
	我希望大家能知道，这是一个被放弃的项目，不再继续。或者因其他性质不再公开继续。
	另：archerag.to将他们的服务版本提升到了 3.0 但该模拟器并不支持这一版本。我可能将在后续修正该问题。SQL也进行了更新
	create database archeage
	import sql
	mysql account archeage password archeage

![avatar](/doc/img/TIM截图20180808170435.png)
![avatar](/doc/img/TIM截图20180808170454.png)
![avatar](/doc/img/TIM截图20180808170527.png)
update log：

2018年4月26日
本期并没有什么改变，只是看了一下中文无法显示问题
找到我的文档》archeage》system.cfg》最后添加一行 locale="zh_cn"  
这样便可显示中文服务器列表
同样也带来了问题，因非正统汉化，游戏内的韩文无法显示，也即使提示信息都变成了？？？？？？
2018年4月18日
修正3.0版本数据传输不正确问题
about:
	因开发阶段，只有2.9和4.0版本，so 没有考虑到3.0版本问题。现在3.0版本放出，数据传输方式介于4.0和2.9之间
	现在已经修复了3.0版本的登录数据不正确问题

	
2018年8月8日
修复复写数据导致token和密码换位问题
优化部分代码

#####################
根据国外大神的处理可进入游戏大厅
具体细节还需等待


2018年4月18日
测试目前状况
调整默认服务器选项为3.0+
修正sql增加token

2018年1月21日
修正服务器列表不支持非英文问题
增加1250验证服务器

2017年11月21日
新增服务器列表显示禁用的种族

	诺亚族
	精灵族
	矮人族
	哈里兰族
	兽灵族
	战魔族

修复token错误时读取线程账户信息导致的报错问题


2017年11月7日15点45分

##说明  Explain

如你所见，我正在对它进行修复工作，我不知道其源码来自何处，但确实可以进行基本的模拟工作的开始。显然这是一个被放弃的项目，很多的功能都没有被实现。我有幸发现这个项目，现在已经开始对其进行基本的修复工作。除了账号的验证部分，我已经修复了服务器列表的显示及请求连接游戏服务器，这是一个成功的开始。但对于游戏的模拟工作量太大，没有客户端的源码我无法更深入的去修复。
<s>现在寻求更多的人来加入这个不可能的项目，同时寻求启动客户端的方法需要完全分离GLYPH平台，因为我直接run archeage.exe是未响应的，我不知如何解决这个问题。</s>
有任何问题可以通过 mail：ahlyl94@gmail.com 联系我
But 我可能很少阅读邮件，不能随时随地回复你

<s>我在源码中引入了第三方AALauncher启动器，最终一切归原作者所有，当然我们后期会替换该启动器</s>
如果你想直接启动请cmd  code ：
```    
    -{g} +auth_ip 127.0.0.1:1237 -uid {id} -token {token}
    
    archeage.exe  -r +auth_ip 127.0.0.1:1237 -uid 1 -token e10adc3949ba59abbe56e057f20f883e
g:

    r：俄罗斯（正常）
    t：美国（失败）
    k：韩国（失败）
    eu：欧盟（失败）
    ···
id：

    数据库中的账号id
    
token：
    
    32长度的16进制字符：e10adc3949ba59abbe56e057f20f883e
```    
本程序的版权 netcastiel 我不知道是一个人还是组织 name，but 修复版权请归属于I，Yanlongli。

当我在尝试修复改程序时发现，很多数据都未定义
我用Glyph 平台的 北美服务器 208.94.25.230 帮助我进行恢复工作
我通过抓包工具获取了两者之间的通讯，然后用模拟器发送了虚假的服务器列表，很荣幸我成功了，但是！！！
当我们在选择服务器后客户端断开了登陆服务器，开始连接游戏服务器，而之间的通讯量之大是我预料之外的，开始明白这不是一个人的工作。
但是我将源代码再次提交，希望来人继续这个项目

## 调试流程

通过Microsoft Visual Studio 2015 + 版本 
编译后修改ArcheAgeLogin 和 ArcheAge

配置MYSQL，账号：root、密码：root，并将ArcheAgeLogin下面的sql导入进去
安装号Glyph 平台的archeage 打开软件根目录Glyph\Library\GlyphLibrary.xml  并修改 约178行左右的IP地址为127.0.0.1 并将该文件设置为只读模式（若想恢复只需关闭只读或直接删除该文件即可）
接下来你就可以继续我的工作了



分析日志：
台服：
2017年11月27日
数据总和   0c00510000   6700//总数据校验 "字符串" // 提示信息  01//结束符（英文去除）
6f000c005100006700e682a8e79a84e5b8b3e8999fe59ba0e98195e58f8de9818ae688b2e7aea1e79086e8a68fe7aba0e88887e6a29de4be8be69585e5819ce6ac8a34e697a52c20e5a682e69c89e79691e5958fe8ab8be6b4bde5aea2e69c8de4b8ade5bf83313a31e8a9a2e5958f2e01

0c  51 0000  登陆失败
0c  0105000000000000 您尚未加入会员，请前往官网加入

0c004c05000000000000   (正常)密码错误登陆失败  但不会终止程序继续运行  //结尾追加00   但还是终止了

