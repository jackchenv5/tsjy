<script setup>
// region 抽屉组件
import { ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { useFacilities } from '#/facility/composables/facilities.js'
import { dayjs, ElMessage } from 'element-plus'
import { addPartMaintainHistoryApi } from '@/apis/sawingPart.js'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = '创建零件维护记录'
    processTime.value = new Date()
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 表单数据
const formData = ref({
  facilityId: null,
  partName: '',
  remark: '',
  reason: '',
  processMethod: '',
  processTime: null,
})
const formRef = ref()
const formRules = ref({
  facilityId: [
    {
      trigger: 'blur',
      required: true,
      message: '请选择设备',
    },
  ],
  partName: [
    {
      trigger: 'blur',
      required: true,
      message: '请输入零件名称',
    },
  ],
  reason: [
    {
      trigger: 'blur',
      required: true,
      message: '请输入维护原因',
    },
  ],
  processMethod: [
    {
      trigger: 'blur',
      required: true,
      message: '请输入处理方法',
    },
  ],
  processTime: [
    {
      trigger: 'blur',
      required: true,
      message: '请选择处理时间',
    },
  ],
})
const processTime = ref()
// endregion

// region 设备列表
const { enabledFacilities } = useFacilities()
// endregion

// region 创建
const onCreateClick = async () => {
  const time = dayjs(processTime.value).unix()
  if (time) {
    formData.value.processTime = time
  } else {
    formData.value.processTime = null
  }
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addPartMaintainHistory()
    }
  })
}
const loadingAddPartMaintainHistory = ref(false)
const addPartMaintainHistory = async () => {
  if (loadingAddPartMaintainHistory.value) {
    return
  }
  loadingAddPartMaintainHistory.value = true
  try {
    await addPartMaintainHistoryApi(formData.value)
    ElMessage({
      type: 'success',
      message: '添加零件维护记录成功',
    })
    formData.value = {
      facilityId: null,
      partName: '',
      remark: '',
      reason: '',
      processMethod: '',
      processTime: null,
    }
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      type: 'error',
      message: '添加零件维护记录失败',
    })
    console.error(e)
  } finally {
    loadingAddPartMaintainHistory.value = false
  }
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :size="drawerSize" :title="drawerTitle">
    <ElForm
      :model="formData"
      label-position="top"
      ref="formRef"
      :rules="formRules"
    >
      <ElFormItem label="设备名称" prop="facilityId">
        <ElSelect v-model="formData['facilityId']">
          <ElOption
            v-for="facility of enabledFacilities"
            :label="facility['name']"
            :value="facility['id']"
            :key="facility['id']"
          >
          </ElOption>
        </ElSelect>
      </ElFormItem>
      <ElFormItem label="零件名称" prop="partName">
        <ElInput v-model="formData.partName"></ElInput>
      </ElFormItem>
      <ElFormItem label="零件备注" prop="remark">
        <ElInput v-model="formData.remark"></ElInput>
      </ElFormItem>
      <ElFormItem label="维护原因" prop="reason">
        <ElInput v-model="formData.reason" type="textarea" :rows="2"></ElInput>
      </ElFormItem>
      <ElFormItem label="处理方法" prop="processMethod">
        <ElInput v-model="formData.processMethod" type="textarea" :rows="2">
        </ElInput>
      </ElFormItem>
      <ElFormItem label="处理时间" prop="processTime">
        <ElDatePicker type="datetime" v-model="processTime" :clearable="false">
        </ElDatePicker>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onCreateClick">创建</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
