<script setup lang="ts">
import { ElMessage, useLocale } from 'element-plus'
import { useDrawer } from '../../common/composables/drawer'
import { ref, watch,onMounted } from 'vue'
import { updateFacilityApi } from '../apis/facility'
// import { uploadFile } from '../apis/facility'
import { Delete, Plus, ZoomIn } from '@element-plus/icons-vue'
import { UploadInstance} from 'element-plus'
import type { UploadFile } from 'element-plus'

// region 组件
const model = defineModel()
const { t } = useLocale()
const props = defineProps(['facility'])
const emits = defineEmits(['finished'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = t('el.facility.setting.updateFacility')
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 处理表单
const formData = ref({
  id: 0,
  name: '',
  serialNumber: '',
  isEnabled: false,
  description: '',
})
watch(
  () => props.facility,
  (value) => {
    if (value) {
      formData.value.id = value.id
      formData.value.name = value.name
      formData.value.serialNumber = value.serialNumber
      formData.value.isEnabled = value.isEnabled
      formData.value.description = value.description
    }
  },
)
const formRef = ref()
const formRules = ref({
  name: [
    {
      required: true,
      message: t('el.facility.setting.requiredMsg'),
      trigger: 'blur',
    },
  ],
})
const loadingUpdateFacility = ref(false)
const updateFacility = async () => {
  loadingUpdateFacility.value = true
  try {
    await updateFacilityApi(formData.value)
    ElMessage({
      message: t('el.facility.setting.updateSuccessful'),
      type: 'success',
    })
    formData.value = {
      id: 0,
      name: '',
      serialNumber: '',
      isEnabled: false,
      description: '',
    }
    emits('finished')
    closeDrawer()
  } catch (e) {
    console.error(e)
    ElMessage({
      message: t('el.facility.setting.updateFailed'),
      type: 'error',
    })
  } finally {
    loadingUpdateFacility.value = false
  }
}
const onUpdate = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await updateFacility()
    }
  })
}
// endregion
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)

// const fileList = ref()

//处理图片卡片的预先渲染
const handlePictureCardPreview = async (file) => {
  dialogImageUrl.value = file.url
  dialogVisible.value = true
}

//对现有图片进行下载，目前无效，可以取消该按钮和功能
// const handleDownload = async (file) => {
//   console.log(file)
//   await uploadFile(file);
// }

//删除图片功能
const handleRemove = () => {
  upload.value?.clearFiles(); // 清空文件列表
};
// const handleRemove = async (file) => { 
//   fileList.value = []; 

//   const formData = new FormData();
//   // 将文件添加到表单对象中
//   formData.append('formFiles', file.raw);

//   await uploadFile(formData);
// }

//const formData = new FormData()
const upload =ref<UploadInstance | null>(null)
const fileListTemp = ref<UploadFile[]>([]);

//更改设备图片触发事件
const handleFileChange = ( file:UploadFile ,newfileListTemp:UploadFile[] ) => {
  // 更新文件列表
  fileListTemp.value = newfileListTemp;
};

//提交按钮逻辑处理
const submitImage = () => {
  if (!upload.value) return;

  const file = fileListTemp.value[0]?.raw;
  if (!file) {
    ElMessage.error('请选择文件');
    return;
  }

  //保存图片到本地缓存，并给出反馈信息
  const reader = new FileReader();
  reader.onload = (e) => {
    const base64 = e.target?.result as string;
    // 保存图片路径到 localStorage
    localStorage.setItem(`facility-${props.facility.id}`, base64);

    ElMessage.success('图片上传成功');
    upload.value?.clearFiles(); // 清空文件列表
    return base64
  };
  closeDrawer()
  reader.readAsDataURL(file);
};

// 初始化加载图片
onMounted(() => {
  // 初始化时加载设备的图片
  const customImgUrl = localStorage.getItem(`facility-${formData.value.id}`);
  if (customImgUrl) {
    // 可以在这里处理加载自定义图片的逻辑
  }
})

//设备视图逻辑，让当前的设备图片初始就加载在设备更新视图中显示
// const imgUrl = '@/assets/JYKJ.png'
// const getImageUrl = (facilityId:number) => {
//   // 从 localStorage 获取设备的图片路径
//   const customImgUrl = localStorage.getItem(`facility-${facilityId}`);
//   return customImgUrl || imgUrl;
// };

// const fileOnChange = (file) => {

//   //下面部分可以对文件进行判断
//   const isIMAGE = (file.raw.type === 'image/jpeg' || file.raw.type === 'image/png' || file.raw.type === 'image/gif');
//   const isLt1M = file.size / 1024 / 1024 < 1;

//   if (!isIMAGE) {
//   	alert('上传文件只能是图片格式!');
//   	return false;
//   }
//   if (!isLt1M) {
//   	alert('上传文件大小不能超过 1MB!');
//   	return false;
//   }
  
//   const reader = new FileReader();
//   reader.onload = (e) => {
//     //将图片转换为Base64格式
//     const newImageUrl = e.target.result
//     //更新Pinia Store中的图片URL
//     imageStore.updateMachineImage(props.facility['id'],newImageUrl);
//   };
//   //读取文件内容
//   reader.readAsDataURL(file.raw);

//   if (file.status === 'ready') {
//     console.log(1)
//     fileListTemp.push(file)
//   }
// }


//内置地址
// let path = `C:\\Users\\Administrator\\Desktop\\图片\\声音`

// const download = () => {
//   console.log(2)
//   window.location.href = `https://localhost:7065/api/File/action/Download?subDirectory=${path}`
  // axios.get(`https://localhost:7065/api/File/action/Download?subDirectory=${path}`).then((res) => {
  // 	console.log(res)
  // 	if (res.status === 200) {
  // 		console.log(res.data.size)
  // 	}
  // })
// }
// const confirm = () => {
//   const formData = new FormData()
//   console.log(formData.has('formFiles'))

//   fileListTemp.forEach((item, index) => {
//     formData.append("formFiles", item.raw)
//     //formData.append("subDirectory", 'file')
//     console.log(item + index)
//     console.log(2)
//   })
//   console.log(formData.has('formFiles'))
//   uploadFiles(formData)
// }

//"https://localhost:7065/api/File/action/Upload?subDirectory=1"
// function uploadFiles(data) {
  // axios.post("https://localhost:5500/api/File/action/Upload?subDirectory=1", data).then((res) => {
  //   console.log(res)
  //   if (res.status === 200) {
  //     console.log(res.data.size)
  //   }
  // })
// }
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm :model="formData" label-position="top" ref="formRef" :rules="formRules">
      <ElFormItem prop="name" :label="t('el.facility.setting.name')" required>
        <ElInput v-model="formData.name"></ElInput>
      </ElFormItem>
      <ElFormItem prop="serialNumber" :label="t('el.facility.setting.serialNumber')">
        <ElInput v-model="formData.serialNumber"></ElInput>
      </ElFormItem>
      <ElFormItem prop="isEnabled" :label="t('el.facility.setting.isEnabled')">
        <ElSwitch v-model="formData.isEnabled"></ElSwitch>
      </ElFormItem>
      <ElFormItem prop="description" :label="t('el.facility.setting.description')">
        <ElInput v-model="formData.description"></ElInput>
      </ElFormItem>
      <ElFormItem label="设备图片">
        <!-- <el-upload
          action="#"
          list-type="picture-card"
          :auto-upload="false"
          :limit=1
          v-model:file-list="fileListTemp"
        > -->
        <el-upload
          ref="upload"
          action="#"
          list-type="picture-card"
          multiple
          :file-list="fileListTemp"
          :auto-upload="false"
          @change="handleFileChange">
          <!--:on-change="fileOnChange" -->
          <!-- <el-button type="primary">上传设备图片</el-button> -->
          <el-icon>
            <Plus />
          </el-icon>

          <template #file="{ file }">
            <div>
              <img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
              <span class="el-upload-list__item-actions">
                <span class="el-upload-list__item-preview" @click="handlePictureCardPreview(file)">
                  <el-icon><zoom-in /></el-icon>
                </span>
                <!-- 下载上传图片，暂无需求不开放 -->
                <!-- <span v-if="!disabled" class="el-upload-list__item-delete" @click="handleDownload(file)">
                  <el-icon><Download /></el-icon>
                </span> -->
                <span v-if="!disabled" class="el-upload-list__item-delete" @click="handleRemove()">
                  <el-icon><Delete /></el-icon>
                </span>
              </span>
            </div>
          </template>
        </el-upload>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onUpdate" :loading="loadingUpdateFacility">
          {{ t('el.facility.setting.update') }}
        </ElButton>
        <el-button type="success" @click="submitImage">更改设备图片</el-button>
      </ElFormItem>

    </ElForm>

    <el-dialog v-model="dialogVisible">
      <img w-full :src="dialogImageUrl" alt="Preview Image" />
    </el-dialog>
  </ElDrawer>
</template>

<style scoped lang="scss">
</style>
