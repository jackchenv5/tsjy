<script setup>
import { ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { deleteMotorApi } from '@/apis/sawingMotor.js'
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
    drawerTitle.value = '删除电机'
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
const loadingDeleteMotor = ref(false)
const onDeleteClick = async () => {
  await deleteMotor()
}
const deleteMotor = async () => {
  if (loadingDeleteMotor.value) {
    return
  }
  loadingDeleteMotor.value = true
  try {
    await deleteMotorApi(motorData.value['id'])
    ElMessage({
      type: 'success',
      message: '删除电机成功',
    })
    closeDrawer()
    emits('finished')
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '删除电机失败',
    })
  } finally {
    loadingDeleteMotor.value = false
  }
}
// endregion
</script>

<template>
  <ElDrawer v-model="showDrawer" :size="drawerSize" :title="drawerTitle">
    <ElForm :model="motorData" label-position="top" @submit.prevent>
      <ElFormItem prop="name" required label="电机名称">
        <ElInput readonly v-model="motorData.name"></ElInput>
      </ElFormItem>
      <ElFormItem>
        <template #label>
          <p class="confirm-text">删除后无法恢复，确定要删除吗？</p>
        </template>
        <ElButton type="danger" @click="onDeleteClick">删除</ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss">
.confirm-text {
  color: var(--el-color-danger);
}
</style>
