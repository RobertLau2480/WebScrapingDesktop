# WebScrapingDesktop
A small tool that can regularly scrape text information from web pages and display it on the desktop.

一个能定时抓取网页文字信息并将其显示在桌面上的小工具。

## 功能特点

- **定时抓取**: 支持最多在一个网页抓取3个XPath的内容
- **窗口置底**: 可切换锁定/解锁状态，锁定时置底且鼠标穿透
- **自定义外观**: 支持修改窗口颜色、透明度，字体类型、大小、粗斜体、颜色
- **抓取完善**: 支持主链接与备用链接，提供冗余
- **占用小**: 不带运行库的版本占用空间不到1MB

## 外观展示
- **外观**
![外观](https://github.com/user-attachments/assets/cfb921b3-7f44-436e-9a68-5603e364731d "外观")

------------


- **设置界面**
![设置](https://github.com/user-attachments/assets/3df91f2b-af2a-44f8-8239-4a41bffd348f "设置")

------------


- **右键菜单**

![右键菜单](https://github.com/user-attachments/assets/abefd423-d2a1-4a60-abf1-d32f65e5bb4b "右键菜单")

### 其他说明
- 所有设置存储在主程序目录下的setting.json中，可手动修改。
- 如果您手动编译，编译后请将logo.ico放入主程序目录，否则将无法启动。
- 本项目借助了DeepSeek编写。
- 如需开机启动，请按Win+R打开“运行”窗口，输入``shell:startup``进入启动文件夹，然后复制一份本程序的快捷方式放到该文件夹中。
