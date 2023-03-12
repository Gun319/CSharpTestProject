$(function () {

    layui.use(['layer', 'util'], function () {
        var layer = layui.layer,
            util = layui.util;

        var upload;

        //上传
        $('#upload').click(function () {

            $('#progress-group').show();


            var fileInput = $('#file').get(0).files[0];

            if (!fileInput) {
                layer.msg("请选择上传文件！");
                return;
            }

            if (upload) {
                layer.msg("已经选择过上传文件啦！");
                return;
            }

            // 创建tus上传对象
            upload = new tus.Upload($('#file')[0].files[0], {
                // 文件服务器上传终结点地址设置
                endpoint: "uploadfile/",
                // 重试延迟设置
                retryDelays: [0, 3000, 5000, 10000, 20000],
                // 附件服务器所需的元数据
                metadata: {
                    name: file.name,
                    contentType: file.type || 'application/octet-stream',
                    emptyMetaKey: ''
                },
                // 回调无法通过重试解决的错误
                onError: function (error) {
                    console.log("Failed because: " + error)
                },
                // 上传进度回调
                onProgress: onProgress,
                // 上传完成后回调
                onSuccess: function () {
                    layer.msg("上传成功，文件名：" + upload.file.name);
                    console.log("Download %s from %s", upload.file.name, upload.url)
                    upload = undefined;
                }
            })

            upload.findPreviousUploads().then((previousUploads) => {

                var chosenUpload = askToResumeUpload(previousUploads);

                if (chosenUpload) {
                    upload.resumeFromPreviousUpload(chosenUpload);
                }

                upload.start();
            });


            // upload.start()
        });

        //暂停
        $('#pause').click(function () {
            if (!upload) {
                layer.msg("请先开始上传！")
                return;
            }
            if (upload._aborted) {
                layer.msg("已经暂停上传啦！")
                return;
            }
            upload.abort()
            console.log(upload._aborted)
        });

        //继续
        $('#continue').click(function () {
            if (!upload) {
                layer.msg("请先开始上传！")
                return;
            }
            if (!upload._aborted) {
                layer.msg("请先暂停上传！")
                return;
            }
            upload.start()
            console.log(upload._aborted)
        });

        function askToResumeUpload(previousUploads) {
            if (previousUploads.length === 0) return null;

            console.log(previousUploads);

            var text = "系统查询到您之前上传过此文件是否恢复上传？:\n\n";
            previousUploads.forEach((previousUpload, index) => {
                text += "[" + index + "] " + util.toDateString(previousUpload.creationTime) + "\n";
            });
            text += "\n输入相应的号码恢复上传或按“取消”键重新上传";

            var answer = prompt(text);
            var index = parseInt(answer, 10);

            if (!isNaN(index) && previousUploads[index]) {
                return previousUploads[index];
            }
        }

        //上传进度展示
        function onProgress(bytesUploaded, bytesTotal) {
            var percentage = (bytesUploaded / bytesTotal * 100).toFixed(2);
            $('#progress').attr('aria-valuenow', percentage);
            $('#progress').css('width', percentage + '%');

            $('#percentage').html(percentage + '%');

            var uploadBytes = byteToSize(bytesUploaded);
            var totalBytes = byteToSize(bytesTotal);

            $('#size').html(uploadBytes + '/' + totalBytes);
        }

        //将字节转换为Byte、KB、MB等
        function byteToSize(bytes, separator = '', postFix = '') {
            if (bytes) {
                const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
                const i = Math.min(parseInt(Math.floor(Math.log(bytes) / Math.log(1024)).toString(), 10), sizes.length - 1);
                return `${(bytes / (1024 ** i)).toFixed(i ? 1 : 0)}${separator}${sizes[i]}${postFix}`;
            }
            return 'n/a';
        }
    });
});