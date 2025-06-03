<script setup>
import { ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { updateMotorBindingApi } from '@/apis/sawingMotor.js'
import { ElMessage } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['motor'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = '更新阈值'  
    motorData.value = props['motor']
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 电机数据
const motorData = ref()
// endregion

// region 更新电机

// const formRef = ref()
// const formRules = ref({
//   partName: [{ trigger: 'blur', required: true, message: '请输入零件名称' }],
// })

const loadingUpdateMotor = ref(false)
const onUpdateClick = async () => { 
  // await formRef.value.validate(async (valid) => {
  //   if (valid) {
  //     await updateMotor()
  //   }
  // }) 
  await updateMotor()
}
const updateMotor = async () => {
  if (loadingUpdateMotor.value) {
    return
  }
  loadingUpdateMotor.value = true
  try {
    await updateMotorBindingApi(motorData.value)
    ElMessage({
      type: 'success',
      message: '更新阈值成功',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '更新阈值失败',
    })
  } finally {
    loadingUpdateMotor.value = false
  }
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :size="drawerSize" :title="drawerTitle">
    <ElForm
      :model="motorData"
      label-position="top"     
    >
      <ElFormItem prop="max" required label="正常上阈值">
        <ElInput v-model="motorData.max"></ElInput>
      </ElFormItem>
      <ElFormItem prop="min" required label="正常下阈值">
        <ElInput v-model="motorData.min"></ElInput>
      </ElFormItem>
      <ElFormItem prop="maxWarning" required label="警告上阈值">
        <ElInput v-model="motorData.maxWarning"></ElInput>
      </ElFormItem>
      <ElFormItem prop="minWarning" required label="警告下阈值">
        <ElInput v-model="motorData.minWarning"></ElInput>
      </ElFormItem>
      <ElFormItem prop="maxAlarm" required label="报警上阈值">
        <ElInput v-model="motorData.maxAlarm"></ElInput>
      </ElFormItem>
      <ElFormItem prop="minAlarm" required label="报警下阈值">
        <ElInput v-model="motorData.minAlarm"></ElInput>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onUpdateClick">更新</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
</style>
