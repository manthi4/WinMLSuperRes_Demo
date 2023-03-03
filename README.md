# WinMLSuperRes_Demo
A demo showing how to integrate superesolution models with Window's ml system

## Windows ML

* Converting models to ONNX
    * https://learn.microsoft.com/en-us/windows/ai/windows-ml/tutorials/tensorflow-convert-model
    * https://onnxruntime.ai/docs/tutorials/tf-get-started.html
    * https://github.com/onnx/tensorflow-onnx#cli-reference
    * TLDR: For ESRGAN I first saved the model as a TFLITE model and then used the following command to convert to ONNX:
        * ``python -m tf2onnx.convert --tflite /content/lite-model_esrgan-tf2_1.tflite --output /content/esrgan_lite_op7_rs6_nchw.onnx --opset 7 --target rs6 --inputs input_0 --inputs-as-nchw input_0 --outputs Identity --outputs-as-nchw Identity``
        *  I then had to open the model using the WinML Dashboard: https://learn.microsoft.com/en-us/windows/ai/windows-ml/dashboard and manually set the input/output types to "Image"
    
   * Only specific ONNX opsets work for WinML, I used opset 7 for this conversion. Info about this: https://learn.microsoft.com/en-us/windows/ai/windows-ml/onnx-versions 
   * Additionally the input/output Images for ESRGAN are ofiginally in the format [Batch, Height, Width, Colors], but WinML expects [Batch, Colors, Height, Width]. These explain all the flags set in the above tf2onnx command.

* Integrating Model into project.

    Actually using the model is simple once it is in the proper ONNX format, just follow this tutorial:
    https://learn.microsoft.com/en-us/windows/ai/windows-ml/get-started-uwp

    * Keep in mind that ImageDataValue objects automatically get scaled and cropped to whatever shape the model expects as input.

## Super Resolution

* Models:
    * HAT-L (2023) - Current Cutting edge
    * SwinIR (2022) - Previous cutting edge model.
        * https://huggingface.co/rocca/swin-ir-onnx
    * ESRGAN (2018) - Basic model used for many demos, but it is old and doesn't have amazing performance. 
        * https://tfhub.dev/captain-pool/esrgan-tf2/1
    