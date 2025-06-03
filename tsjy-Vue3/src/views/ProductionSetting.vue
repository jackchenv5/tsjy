<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import {
  getProductionBindingsApi,
  updateProductionBindingApi,
} from '@/apis/sawingProduction.js'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import UnbindingVariable from '#/variable/components/UnbindingVariable.vue'

// region 选中设备
const currentFacility = ref()
const facilityChange = async (facility) => {
  currentFacility.value = facility
  await getBindings()
}
// endregion

// region 获取绑定数据
const loadingBindings = ref(false)
const bindings = ref([])
const getBindings = async () => {
  loadingBindings.value = true
  try {
    const { data } = await getProductionBindingsApi(currentFacility.value['id'])
    bindings.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingBindings.value = false
  }
}
const getBindingTypeString = (bindingType) => {
  switch (bindingType) {
    case 1:
      return '客户代码'
    case 2:
      return '材料规格'
    case 3:
      return '开始切割信号'
    case 4:
      return '切割完成信号'
  }
}
// endregion

// region 更新绑定
const loadingUpdateBinding = ref(false)
const updateBinding = async (binding) => {
  loadingUpdateBinding.value = true
  try {
    await updateProductionBindingApi(binding)
    await getBindings()
  } catch (e) {
    console.error(e)
  } finally {
    loadingUpdateBinding.value = false
  }
}
// endregion

// region 绑定
const showSelectVariable = ref(false)
const currentBinding = ref()
const onBinding = (binding) => {
  currentBinding.value = binding
  showSelectVariable.value = true
}
const onVariableSelect = async (value) => {
  const binding = {
    id: currentBinding.value['id'],
    connectorInstance: value['connectorInstance'],
    connectionName: value['connectionName'],
    dataPoint: value['dataPointName'],
    name: value['name'],
  }
  await updateBinding(binding)
  currentBinding.value = null
}
// endregion

// region 解绑
const showUnbindingConfirm = ref(false)
const onConfirmUnbinding = async () => {
  const binding = {
    id: currentBinding.value['id'],
    connectorInstance: '',
    connectionName: '',
    dataPoint: '',
    name: '',
  }
  await updateBinding(binding)
}
const onUnbinding = async (binding) => {
  currentBinding.value = binding
  showUnbindingConfirm.value = true
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>生产设置</template>
    <template #default>
      <FacilityList @change="facilityChange"></FacilityList>
      <ElTable
        :data="bindings"
        v-loading="loadingBindings || loadingUpdateBinding"
        class="table"
        show-overflow-tooltip
        stripe
      >
        <ElTableColumn label="序号" type="index" width="80"></ElTableColumn>
        <ElTableColumn label="绑定类型" prop="bindingType" width="160">
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
        <ElTableColumn width="170">
          <template #default="{ row }">
            <ElButtonGroup size="small">
              <ElButton type="primary" plain @click="onBinding(row)">
                绑定
              </ElButton>
              <ElButton type="warning" plain @click="onUnbinding(row)">
                解绑
              </ElButton>
            </ElButtonGroup>
          </template>
        </ElTableColumn>
      </ElTable>
    </template>
  </ContentLayout>
  <SelectVariable v-model="showSelectVariable" @select="onVariableSelect">
  </SelectVariable>
  <UnbindingVariable
    v-model="showUnbindingConfirm"
    @finished="onConfirmUnbinding"
  >
  </UnbindingVariable>
</template>

<style scoped lang="scss"></style>
