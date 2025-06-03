<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { nextTick, ref } from 'vue'
import {
  getStatusBindingsApi,
  updateStatusBindingsApi,
} from '../apis/statusBinding'
import FacilityList from '#/facility/components/FacilityList.vue'
import { useLocale } from 'element-plus'
import SelectVariable from '#/variable/components/SelectVariable.vue'
import UnbindingVariable from '#/variable/components/UnbindingVariable.vue'

// region 组件
const { t } = useLocale()
// endregion

// region 状态绑定数据
const currentFacility = ref()
const onFacilityChange = (facility) => {
  currentFacility.value = facility
  nextTick(async () => {
    await getStatusBindings()
  })
}
const loadingStatusBindings = ref(false)
const statusBindings = ref([])
const getStatusBindings = async () => {
  loadingStatusBindings.value = true
  try {
    const { data } = await getStatusBindingsApi(currentFacility.value['id'])
    statusBindings.value = data
  } catch (error) {
    console.error(error)
  } finally {
    loadingStatusBindings.value = false
  }
}
const getBindingTypeString = (bindingType) => {
  /*
      Invalid = 0,
    Running = 1,
    Standby = 2,
    Stopped = 3,
    Error = 4
   */
  switch (bindingType) {
    case 0:
      return t('el.facilityStatus.statusBinding.invalid')
    case 1:
      return t('el.facilityStatus.statusBinding.running')
    case 2:
      return t('el.facilityStatus.statusBinding.standby')
    case 3:
      return t('el.facilityStatus.statusBinding.stopped')
    case 4:
      return t('el.facilityStatus.statusBinding.error')
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
  await updateStatusBinding(binding)
  currentBinding.value = null
}
const showUnbindingConfirm = ref(false)
const onConfirmUnbinding = async () => {
  const binding = {
    id: currentBinding.value['id'],
    connectorInstance: '',
    connectionName: '',
    dataPoint: '',
    name: '',
  }
  await updateStatusBinding(binding)
}
const onUnbinding = async (binding) => {
  currentBinding.value = binding
  showUnbindingConfirm.value = true
}
const loadingUpdateStatusBinding = ref(false)
const updateStatusBinding = async (binding) => {
  loadingUpdateStatusBinding.value = true
  try {
    await updateStatusBindingsApi(binding)
    await getStatusBindings()
  } catch (e) {
    console.error(e)
  } finally {
    loadingUpdateStatusBinding.value = false
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityStatus.statusBinding.statusBinding') }}
    </template>
    <template #default>
      <div class="content-container">
        <FacilityList @change="onFacilityChange"></FacilityList>
        <ElTable
          :data="statusBindings"
          v-loading="loadingStatusBindings || loadingUpdateStatusBinding"
          class="table"
        >
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.index')"
            type="index"
            width="80"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.bindingType')"
            prop="bindingType"
            width="120"
          >
            <template #default="{ row }">
              {{ getBindingTypeString(row['bindingType']) }}
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.connectorInstance')"
            prop="connectorInstance"
            width="170"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.connectionName')"
            prop="connectionName"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.dataPointName')"
            prop="dataPoint"
            width="150"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityStatus.statusBinding.variableName')"
            prop="name"
            min-width="240"
          >
          </ElTableColumn>
          <ElTableColumn width="170">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton type="primary" plain @click="onBinding(row)">
                  {{ t('el.facilityStatus.statusBinding.binding') }}
                </ElButton>
                <ElButton type="warning" plain @click="onUnbinding(row)">
                  {{ t('el.facilityStatus.statusBinding.unbinding') }}
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
      </div>
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

<style scoped lang="scss">
.content-container {
  flex: 1;
  display: flex;
  flex-flow: column;

  .table {
    flex: 1;
  }
}
</style>
