1. 关键部署代码路径：
8-完整项目\C#Code\origin_0409_new\origin\ScopeX.Core\Model\ArtificialIntelligence\IntelligentNoiseReductionModel.cs

2. 关键部署C#代码：
IntelligentNoiseReductionModel.cs（部署层面只有这个最重要，本文件跟项目中其他代码粘连不大）（winSize = 240，count = 100）

3. 另外重要：
8-完整项目\C#Code\origin_0409_new\origin\ScopeX.U2\bin\Debug\net6.0-windows\AI\TensorModels
这个地址下面放了已有的onnx文件，IntelligentNoiseReductionModel.cs调用onnx就是从这个文件夹里调用。
所以日后若训练生成的onnx文件可以放到这个文件夹里。

4. 两个文件说明：
origin_0401_linshi项目，是我用来测试推理用时的，所以此项目下面的IntelligentNoiseReductionModel.cs文件会在调试台输出端输出推理用时、资源占用等等，用于测试能耗。
origin_0409_new.7z项目，是智能示波器截至今日4月15日正在使用的最新代码。其中的IntelligentNoiseReductionModel.cs保持原逻辑，没有加入计算推理用时的逻辑，是主要使用的版本。日后使用直接解压这个zip文件使用。