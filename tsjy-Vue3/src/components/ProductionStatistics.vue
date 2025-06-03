<script setup>
import { inject, nextTick, onMounted, ref, watch } from 'vue'
import { dayjs } from 'element-plus'
import { getProductionStatisticsApi } from '@/apis/sawingProduction.js'

// region 组件

const startTime = inject('startTime')
const endTime = inject('endTime')
const currentFacility = inject('currentFacility')
onMounted(async () => {
  await nextTick(async () => {
    await getProductionStatistics()
  })
})
const model = defineModel()
watch(model, async (value) => {
  if (value) {
    await getProductionStatistics()
    model.value = false
  }
})
// endregion

// region 统计数据
const statisticsData = ref()
const loadingGetProductionStatistics = ref(false)
const getProductionStatistics = async () => {
  if (loadingGetProductionStatistics.value) {
    return
  }
  loadingGetProductionStatistics.value = true
  try {
    const dto = {
      startTime: dayjs(startTime.value).unix(),
      endTime: dayjs(endTime.value).unix(),
      facilityId: currentFacility.value.id,
    }
    const { data } = await getProductionStatisticsApi(dto)
    statisticsData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetProductionStatistics.value = false
  }
}
// endregion
</script>

<template>
  <div class="container">
    <div class="header">生产统计</div>
    <div class="content-container">
      <div
        class="customer"
        v-for="(statisticsItem, index) of statisticsData"
        :key="index"
      >
        <div class="customer-header">
          <div class="name">
            {{ '客户：' + statisticsItem.customerCode }}
          </div>
          <div class="count">
            {{ statisticsItem['totalCount'] + ' 版' }}
          </div>
        </div>
        <div class="customer-specifications">
          <div
            class="specification"
            v-for="(spec, index) of statisticsItem['materialSpecifications']"
            :key="index"
          >
            <div class="name">
              {{ spec['name'] }}
            </div>
            <div class="count">
              {{ spec['count'] + ' 版' }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.container {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);
  flex: 1;
  display: flex;
  flex-flow: column;
  overflow: auto;

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 1px solid var(--el-border-color);
  }

  > .content-container {
    flex: 1;
    padding: 1rem;
    overflow: auto;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(24rem, 1fr));
    //grid-auto-rows: 20rem;
    grid-gap: 1rem;

    > .customer {
      border: 1px solid var(--el-border-color);
      padding: 0 1rem;
      border-radius: var(--el-border-radius-base);
      display: flex;
      flex-flow: column;

      &:hover {
        box-shadow: var(--el-box-shadow-light);
      }

      > .customer-header {
        padding: 1rem;
        border-bottom: 1px solid var(--el-border-color);
        font-weight: bold;
        display: flex;
        flex-flow: row;
        justify-content: space-between;
      }

      > .customer-specifications {
        flex: 1;
        display: flex;
        overflow: auto;
        flex-flow: column;
        grid-gap: 0.5rem;
        padding: 1rem 0;

        > .specification {
          display: flex;
          flex-flow: row;
          grid-gap: 1rem;
          justify-content: space-between;
          padding: 0.5rem 1rem;
          background-color: var(--el-color-primary-light-8);
          border-radius: var(--el-border-radius-base);
        }
      }
    }
  }
}
</style>
