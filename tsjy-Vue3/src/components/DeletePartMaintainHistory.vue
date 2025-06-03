<script setup>
import { computed, ref, watch } from 'vue'
import { useDrawer } from '#/common/composables/drawer.js'
import { ElMessage,dayjs} from 'element-plus'
import { deletePartHistoriesApi } from '@/apis/sawingPart.js'
import advancedFormat from 'dayjs/plugin/advancedFormat'

// region 组件
const model = defineModel()
const emits = defineEmits(['finished'])
const props = defineProps(['part'])

// endregion

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
watch(model, (val) => {
  if (val) {
    partFormData.value = props.part
    drawerTitle.value = '删除零件维护记录'
    openDrawer()
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
// endregion

// region 零件数据
const partFormData = ref()
// endregion

// region 删除零件
const loadingDeletePart = ref(false)
const onDeleteClick = async () => {
  await deletePart()
}
const deletePart = async () => {
  if (loadingDeletePart.value) {
    return
  }
  loadingDeletePart.value = true
  try {
    await deletePartHistoriesApi(partFormData.value.id)
    ElMessage({
      type: 'success',
      message: '删除零件成功',
    })
    partFormData.value = {
      partName: '',
      facilityId: null,
      remark: '',
      connectorInstance: '',
      connectionName: '',
      dataPoint: '',
      variableName: '',
    }
    closeDrawer()
    emits('finished')
  } catch (e) {
    ElMessage({
      type: 'error',
      message: '删除零件失败',
    })
    console.error(e)
  } finally {
    loadingDeletePart.value = false
  }
}
// endregion

</script>

<template>
  <ElDrawer v-model="showDrawer" :size="drawerSize" :title="drawerTitle">
    <ElForm :model="partFormData" label-position="top" @submit.prevent>
     
      <ElFormItem prop="partName" label="零件名称" readonly>
        <ElInput v-model="partFormData.partName" ></ElInput>
      </ElFormItem>
      <ElFormItem label="零件备注" prop="remark" readonly >
        <ElInput v-model="partFormData.remark"></ElInput>
      </ElFormItem>
      <ElFormItem label="维护原因" prop="reason" readonly>
        <ElInput v-model="partFormData.reason" type="textarea" :rows="2" ></ElInput>
      </ElFormItem>
      <ElFormItem label="处理方法" prop="processMethod" readonly>
        <ElInput v-model="partFormData.processMethod" type="textarea" :rows="2" >
        </ElInput>
      </ElFormItem>
      <ElFormItem label="处理时间" prop="processTime" readonly>
        <ElDatePicker type="datetime" value-format="X" v-model="partFormData.processTime" :clearable="false">
        </ElDatePicker>
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
