01.Framework
	存放项目框架模块。
11.Service
	存放后台服务，SPF核心功能块，前端不可直接访问。
21.Presentation
	前端模块，界面展示以及业务逻辑等
	ProjectExtension：前端扩展模块。
		XLY.SF.Project.Extension：主要用于存储当前项目公用属性，以及前台扩展辅助功能，等等。如果有其他前台公用功能或者公用属性、扩展方法之类的都存放到这个工程里面。
		XLY.SF.Project.ProxyService：主要是写和后台服务打交到的功能，以及服务调用都写到这里。
		XLY.SF.Project.ViewDomain：视图程序域，存放界面用的Model以及前台所有使用到的Model等，不与任何逻辑相关。


	Theme：前端样式模块
	View：前端展示界面模块
		此模块分类原则:Main文件夹下存放所有主界面显示的View


	ViewModle：界面功能逻辑模块
		此模块分类原则:Main文件夹下存放所有主界面显示的ViewModel

23.Shell
	程序启动模块，程序错误监听导航、组合部件等功能，与业务逻辑无关