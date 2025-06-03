<script setup>
import { ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { updateMotorApi } from '@/apis/sawingMotor.js'
import { ElMessage } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['facilityId', 'motor'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = '更新电机'
    motorData.value.facilityId = props.facilityId
    motorData.value = props['motor']
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 电机数据
const motorData = ref({
  facilityId: 0,
  name: '',
})
// endregion

// region 更新电机
const formRef = ref()
const formRules = ref({
  name: [{ trigger: 'blur', required: true, message: '请输入电机名称' }],
})
const loadingUpdateMotor = ref(false)
const onUpdateClick = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await updateMotor()
    }
  })
}
const updateMotor = async () => {
  if (loadingUpdateMotor.value) {
    return
  }
  loadingUpdateMotor.value = true
  try {
    await updateMotorApi(motorData.value)
    ElMessage({
      type: 'success',
      message: '更新电机成功',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '更新电机失败',
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
      ref="formRef"
      :rules="formRules"
      @submit.prevent
    >
      <ElFormItem prop="name" required label="电机名称">
        <ElInput v-model="motorData.name"></ElInput>
      </ElFormItem>
      <ElFormItem>
        <ElButton type="primary" @click="onUpdateClick">更新</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
