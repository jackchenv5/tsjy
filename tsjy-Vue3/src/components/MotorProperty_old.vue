<script setup>
import { onMounted, ref, watch } from 'vue'
import {
  getMotorBindingApi,
  updateMotorBindingApi,
} from '@/apis/sawingMotor.js'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import { ElMessage } from 'element-plus'
import UnbindingVariable from '#/variable/components/UnbindingVariable.vue'

// region 组件
const props = defineProps(['motor'])
watch(
  () => props.motor,
  async (newVal) => {
    if (newVal) {
      await getMotorBindingData()
    }
  },
)
onMounted(async () => {
  await getMotorBindingData()
})
// endregion

// region 获取数据
const loadingGetMotorBinding = ref(false)
const motorBindingData = ref()
const getMotorBindingData = async () => {
  if (loadingGetMotorBinding.value) {
    return
  }
  loadingGetMotorBinding.value = true
  try {
    const { data } = await getMotorBindingApi(props.motor['id'])
    motorBindingData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMotorBinding.value = false
  }
}
const getBindingTypeString = (type) => {
  switch (type) {
    case 1:
      return '轴故障代码'
    case 2:
      return '驱动故障代码'
    case 3:
      return '转速'
    case 4:
      return '转矩'
  }
}
// endregion

// region 绑定/解绑
const showSelectVariable = ref(false)
const currentBinding = ref()
const bindingClick = (row) => {
  currentBinding.value = row
  showSelectVariable.value = true
}
const onVariableSelected = async (value) => {
  const binding = {
    id: currentBinding.value['id'],
    connectorInstance: value['connectorInstance'],
    connectionName: value['connectionName'],
    dataPoint: value['dataPointName'],
    name: value['name'],
  }
  await updateMotorBinding(binding)
}
const loadingUpdateMotorBinding = ref(false)
const updateMotorBinding = async (binding) => {
  loadingUpdateMotorBinding.value = true
  try {
    await updateMotorBindingApi(binding)
    ElMessage({
      type: 'success',
      message: '电机绑定更新成功',
    })
    await getMotorBindingData()
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '电机绑定更新失败',
    })
  } finally {
    loadingUpdateMotorBinding.value = false
  }
}
const showUnbindingConfirm = ref(false)
const unbindingClick = (row) => {
  currentBinding.value = row
  showUnbindingConfirm.value = true
}
const onUnbindingConfirmed = async () => {
  loadingUpdateMotorBinding.value = true
  try {
    const binding = {
      id: currentBinding.value['id'],
      connectorInstance: '',
      connectionName: '',
      dataPoint: '',
      name: '',
    }
    await updateMotorBinding(binding)
  } catch (e) {
    console.error(e)
  } finally {
    loadingUpdateMotorBinding.value = false
  }
}
// endregion
</script>

<template>
  <div class="container">
    <ElTable
      class="table"
      stripe
      show-overflow-tooltip
      :data="motorBindingData"
    >
      <ElTableColumn label="序号" type="index" width="80"></ElTableColumn>
      <ElTableColumn label="绑定类型" prop="bindingType" min-width="160">
        <template #default="{ row }">
          {{ getBindingTypeString(row['bindingType']) }}
        </template>
      </ElTableColumn>
      <ElTableColumn label="连接器实例" prop="connectorInstance" width="170">
      </ElTableColumn>
      <ElTableColumn label="连接名称" prop="connectionName" min-width="160">
      </ElTableColumn>
      <ElTableColumn label="数据点名称" prop="dataPoint" width="150">
      </ElTableColumn>
      <ElTableColumn label="变量名称" prop="name" min-width="240">
      </ElTableColumn>
      <ElTableColumn width="140">
        <template #default="{ row }">
          <ElButtonGroup size="small">
            <ElButton type="primary" plain @click="bindingClick(row)">
              绑定
            </ElButton>
            <ElButton type="warning" plain @click="unbindingClick(row)">
              解绑
            </ElButton>
          </ElButtonGroup>
        </template>
      </ElTableColumn>
    </ElTable>
  </div>
  <SelectVariable v-model="showSelectVariable" @select="onVariableSelected">
  </SelectVariable>
  <UnbindingVariable
    v-model="showUnbindingConfirm"
    @click="onUnbindingConfirmed"
  >
  </UnbindingVariable>
</template>

<style scoped lang="scss">
.container {
  flex: 1;
  display: flex;
  flex-flow: column;

  .table {
    flex: 1;
  }
}
</style>
