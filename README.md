# UnitySapi
UnitySapi是一个Unity可以调用Sapi的工具。

语音开发主要是两种技术：语音识别和语音合成
Sapi是微软提供的语音引擎，win7之后系统自带，效率和质量都很好。
使用Sapi需要使用System.Speech.dll文件，因此在unity中需要拷贝这个文件到Assets目录下，但是会出现无法初始化的错误。
因此，创建一个专门调用Sapi的控制台程序。然后在Unity中和这个控制台程序进行数据交互，以达到在Unity调用Sapi的目的。

#程序工作流程
Unity使用Socket发送语音文本给控制台程序Speech，然后Speech调用Sapi进行语音合成。

#未来功能
未来将加入语音识别功能，unity获得的语音通过该工具得到识别后的文本并显示出来

2016/8/13 由于unity3d已经增加了语音识别功能，本项目不再增加该功能

备注：win10 64位操作系统测试通过，其他系统未测试
