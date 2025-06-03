<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { getPartApi } from '@/apis/sawingPart.js'
import { dayjs } from 'element-plus'

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
        <ElTable class="table" :data="parts" stripe show-overflow-tooltip>
          <ElTableColumn label="零件" prop="partName"></ElTableColumn>
          <ElTableColumn label="备注" prop="remark"></ElTableColumn>
          <ElTableColumn label="使用寿命" prop="lastValue">
            <template #default="{ row }">
              <ElTag
                type="primary"
                size="small"
                v-if="row['lastValue']"
                effect="plain"
              >
                {{ row['lastValue'].toFixed(3) }}
              </ElTag>
              <ElText v-else>-</ElText>
            </template>
          </ElTableColumn>
          <ElTableColumn label="更新时间" prop="updatedAt" width="180">
            <template #default="{ row }">
              <ElTag type="info" size="small" v-if="row['updatedAt']">
                {{ dayjs.unix(row['updatedAt']).format('YYYY-MM-DD HH:mm:ss') }}
              </ElTag>
              <ElText v-else>-</ElText>
            </template>
          </ElTableColumn>
        </ElTable>
      </div>
    </template>
  </ContentLayout>
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
