<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { onBeforeUnmount, onMounted, ref } from 'vue'
import AddPart from '@/components/AddPart.vue'
import { getPartApi } from '@/apis/sawingPart.js'
import UpdatePart from '@/components/UpdatePart.vue'
import DeletePart from '@/components/DeletePart.vue'
// region 组件
onMounted(() => {
  startInterval()
})
onBeforeUnmount(() => {
  stopInterval()
})
// endregion

// region 当前设备
const currentFacility = ref()
const onFacilityChange = async (facility) => {
  currentFacility.value = facility
  await getParts()
}
// endregion

// region 添加零件对话框
const showAddPart = ref(false)
const onAddPartClick = () => {
  showAddPart.value = true
}
const onAddPartFinished = async () => {
  await getParts()
}
// endregion

// region 零件列表
const loadingGetParts = ref(false)
const parts = ref()
const getParts = async () => {
  loadingGetParts.value = true
  try {
    const { data } = await getPartApi(currentFacility.value.id)
    parts.value = data
  } finally {
    loadingGetParts.value = false
  }
}
// endregion

// region 更新，删除零件
const currentPart = ref()
const showUpdatePart = ref(false)
const onUpdatePartClick = (row) => {
  currentPart.value = JSON.parse(JSON.stringify(row))
  showUpdatePart.value = true
}
const onUpdatePartFinished = async () => {
  await getParts()
}

const showDeletePart = ref(false)
const onDeletePartClick = (row) => {
  currentPart.value = JSON.parse(JSON.stringify(row))
  showDeletePart.value = true
}
const onDeletePartFinished = async () => {
  await getParts()
}
// endregion

// region 自动刷新
let intervalId
const startInterval = () => {
  stopInterval()
  intervalId = setInterval(async () => {
    if (currentFacility.value) {
      await getParts()
    }
  }, 5000)
}
const stopInterval = () => {
  if (intervalId) {
    clearInterval(intervalId)
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>零件列表</template>
    <template #default>
      <FacilityList @change="onFacilityChange"></FacilityList>
      <div class="wrapper">
        <div class="header">
          <ElButton type="primary" plain @click="onAddPartClick">
            添加零件
          </ElButton>
        </div>
        <ElTable class="table" :data="parts" stripe show-overflow-tooltip>
          <ElTableColumn label="零件" prop="partName"></ElTableColumn>
          <ElTableColumn label="备注" prop="remark"></ElTableColumn>
          <ElTableColumn
            label="连接器实例"
            prop="connectorInstance"
            width="120"
          >
          </ElTableColumn>
          <ElTableColumn label="连接名称" prop="connectionName" min-width="120">
          </ElTableColumn>
          <ElTableColumn label="数据点名称" prop="dataPoint" width="120">
          </ElTableColumn>
          <ElTableColumn label="变量名称" prop="variableName" min-width="200">
          </ElTableColumn>
          <ElTableColumn width="170">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton type="primary" plain @click="onUpdatePartClick(row)">
                  更新
                </ElButton>
                <ElButton type="danger" plain @click="onDeletePartClick(row)">
                  删除
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
      </div>
    </template>
  </ContentLayout>

  <AddPart
    v-model="showAddPart"
    @finished="onAddPartFinished"
    :facility-id="currentFacility?.id"
  >
  </AddPart>

  <UpdatePart
    v-model="showUpdatePart"
    :part="currentPart"
    @finished="onUpdatePartFinished"
  >
  </UpdatePart>

  <DeletePart
    v-model="showDeletePart"
    :part="currentPart"
    @finished="onDeletePartFinished"
  >
  </DeletePart>
</template>

<style scoped lang="scss">
.wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  grid-gap: 1rem;
  overflow: auto;

  > .table {
    flex: 1;
  }
}
</style>
