添加的所有DLL需要拷贝到DllServerHost目录下【CopyDll工具已经实现该功能，运行即可】
【调试模式】
	1. 需多启动调试【避免附加进程】
	2. 保证DllServerHost为第一个启动

1. 以管理员权限启动DllServerHost
2. 更新DllClient服务引用
3. 如果是以Shell作为启动主程序，需要将DllClient中的app.config文件中ServiceModel节点拷贝到Shell工程中的app.config文件中。同时删除DllClient中的节点