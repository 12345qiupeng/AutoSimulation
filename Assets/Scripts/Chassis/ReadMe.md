# 底盘控制模块

这个模块用于控制机器人底盘的运动和基于前端的路径生成，并与算法交互。

## 底盘控制

底盘有两个运动模式：

* 显示模式

  显示模式下，底盘接受算法发来的位姿并显示。

* 仿真模式

  仿真模式下，底盘可使用键盘方向键控制，并将位姿发送给算法。

## 算法交互

仿真负责启动一个激发器，以启动与算法的交互。