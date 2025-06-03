<script setup>
import { ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { addMotorApi } from '@/apis/sawingMotor.js'
import { ElMessage } from 'element-plus'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['facilityId'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = '添加电机'
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
watch(
  () => props.facilityId,
  (val) => {
    motorData.value.facilityId = val
  },
)
// endregion

// region 添加电机
const formRef = ref()
const formRules = ref({
  name: [{ trigger: 'blur', required: true, message: '请输入电机名称' }],
})
const loadingAddMotor = ref(false)
const addClick = async () => {
  await formRef.value.validate(async (valid) => {
    if (valid) {
      await addMotor()
    }
  })
}
const addMotor = async () => {
  if (loadingAddMotor.value) {
    return
  }
  loadingAddMotor.value = true
  try {
    await addMotorApi(motorData.value)
    ElMessage({
      type: 'success',
      message: '添加电机成功',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '添加电机失败',
    })
  } finally {
    loadingAddMotor.value = false
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
        <ElButton type="primary" @click="addClick">添加</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
