<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { computed, nextTick, ref } from 'vue'
import { useLocale } from 'element-plus'
import SelectPartFile from '../components/SelectPartFile.vue'
import {
  getFacilityPartsApi,
  importFacilityPartApi,
} from '../apis/facilityPart'
import MaintainPlan from '../components/MaintainPlan.vue'

// region 组件
const { t } = useLocale()
// endregion

// region 选择的设备变化
const currentFacility = ref()
const onFacilityChange = async (value) => {
  currentFacility.value = value
  if (currentFacility.value) {
    await getFacilityParts()
  }
}
// endregion

// region 导入
const loadingImport = ref(false)
const onPartDataChange = async (partData) => {
  loadingImport.value = true
  try {
    await importFacilityPartApi(partData)
    await getFacilityParts()
  } catch (e) {
    console.error(e)
  } finally {
    loadingImport.value = false
  }
}

// endregion

// region 树形数据
const treeRef = ref()
const treeData = ref([])
const loadingData = ref(false)
const getFacilityParts = async () => {
  loadingData.value = true
  try {
    const { data } = await getFacilityPartsApi(
      currentFacility.value.id,
      nameFilter.value,
      partTypeFilter.value,
    )
    treeData.value = data
    if (treeData.value.length) {
      await nextTick(() => {
        treeRef.value.setCurrentKey(treeData.value[0].id)
      })
    } else {
      currentPart.value = null
    }
  } catch (e) {
    console.error(e)
  } finally {
    loadingData.value = false
  }
}
const onCurrentChange = (data) => {
  currentPart.value = data
}
const emptyData = computed(() => {
  return treeData.value.length === 0 && !filterState.value
})
// endregion

// region 选中部件数据
const currentPart = ref()
const getPartTypeString = (type) => {
  // 固件，耐用件 - 0
  // 易损件 - 1
  switch (type) {
    case 0:
      return t('el.facilityMaintain.partDetail.durablePart')
    case 1:
      return t('el.facilityMaintain.partDetail.wearingPart')
    default:
      return t('el.facilityMaintain.partDetail.undefinedPart')
  }
}
// endregion

// region 筛选
const filterState = ref(false)
const nameFilter = ref()
const partTypeFilter = ref()
const onFilter = async () => {
  filterState.value = !!(nameFilter.value || partTypeFilter.value)
  await getFacilityParts()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityMaintain.partDetail.partDetail') }}
    </template>
    <template #default>
      <div class="content-container">
        <FacilityList @change="onFacilityChange"></FacilityList>
        <div class="detail-container" v-loading="loadingImport || loadingData">
          <div v-if="!emptyData" class="no-empty-part-container">
            <div class="top-bar">
              <div class="filter">
                <ElInput
                  :placeholder="t('el.facilityMaintain.partDetail.name')"
                  style="width: 20rem"
                  v-model="nameFilter"
                  clearable
                >
                </ElInput>
                <ElSelect
                  style="width: 16rem"
                  v-model="partTypeFilter"
                  clearable
                >
                  <ElOption
                    :value="0"
                    :label="t('el.facilityMaintain.partDetail.durablePart')"
                  >
                  </ElOption>
                  <ElOption
                    :value="1"
                    :label="t('el.facilityMaintain.partDetail.wearingPart')"
                  >
                  </ElOption>
                </ElSelect>
                <ElButton type="primary" plain @click="onFilter">
                  {{ t('el.facilityMaintain.partDetail.filter') }}
                </ElButton>
              </div>
              <SelectPartFile
                v-if="currentFacility"
                :facility-id="currentFacility['id']"
                @change="onPartDataChange"
              >
                <ElButton type="primary" plain>
                  {{ t('el.facilityMaintain.partDetail.reimport') }}
                </ElButton>
              </SelectPartFile>
            </div>
            <div class="data-container">
              <div class="tree-container">
                <ElTree
                  :data="treeData"
                  :expand-on-click-node="false"
                  node-key="id"
                  highlight-current
                  :props="{
                    label: 'name',
                  }"
                  :default-expanded-keys="[treeData[0]?.id]"
                  ref="treeRef"
                  @current-change="onCurrentChange"
                  default-expand-all
                >
                </ElTree>
              </div>
              <div class="table-container" v-if="currentPart">
                <div class="current-part">
                  <ElDescriptions
                    border
                    size="small"
                    :column="4"
                    direction="vertical"
                    v-if="currentPart"
                    :title="t('el.facilityMaintain.partDetail.currentPart')"
                  >
                    <ElDescriptionsItem
                      :label="t('el.facilityMaintain.partDetail.name')"
                      min-width="240"
                    >
                      {{ currentPart.name }}
                    </ElDescriptionsItem>
                    <ElDescriptionsItem
                      :label="t('el.facilityMaintain.partDetail.count')"
                      min-width="80"
                    >
                      {{ currentPart.count }}
                    </ElDescriptionsItem>
                    <ElDescriptionsItem
                      :label="t('el.facilityMaintain.partDetail.partType')"
                      min-width="80"
                    >
                      {{ getPartTypeString(currentPart.partType) }}
                    </ElDescriptionsItem>
                    <ElDescriptionsItem
                      :label="t('el.facilityMaintain.partDetail.treePath')"
                      min-width="100"
                    >
                      {{ currentPart['treePath'] }}
                    </ElDescriptionsItem>
                    <ElDescriptionsItem
                      :label="t('el.facilityMaintain.partDetail.description')"
                      :span="3"
                    >
                      {{
                        currentPart.description ? currentPart.description : '-'
                      }}
                    </ElDescriptionsItem>
                    <ElDescriptionsItem
                      :label="
                        t(
                          'el.facilityMaintain.partDetail.maintainCycleWithUnit',
                        )
                      "
                    >
                      {{
                        currentPart['maintainCycle']
                          ? currentPart['maintainCycle']
                          : '-'
                      }}
                    </ElDescriptionsItem>
                  </ElDescriptions>
                  <MaintainPlan :part="currentPart"></MaintainPlan>
                </div>
                <div class="children-parts"></div>
              </div>
            </div>
          </div>
          <div v-else class="empty-part-container">
            <ElText>设备部件为空，请先</ElText>
            <SelectPartFile
              v-if="currentFacility"
              :facility-id="currentFacility['id']"
              @change="onPartDataChange"
            >
              <ElLink type="primary" :underline="false">
                {{ t('el.facilityMaintain.partDetail.import') }}
              </ElLink>
            </SelectPartFile>
          </div>
        </div>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.content-container {
  display: flex;
  flex-flow: column;
  height: 100%;

  > .detail-container {
    flex: 1;
    display: flex;
    overflow: auto;

    > .no-empty-part-container {
      flex: 1;
      display: flex;
      flex-flow: column;
      grid-gap: 1rem;
      overflow: auto;

      > .top-bar {
        display: flex;
        flex-flow: row wrap;
        grid-gap: 1rem;

        > .filter {
          flex: 1;
          display: flex;
          flex-flow: row wrap;
          grid-gap: 1rem;
        }
      }

      > .data-container {
        flex: 1;
        display: flex;
        flex-flow: row;
        grid-gap: 1rem;
        overflow: auto;

        > .tree-container {
          width: 30rem;
          overflow: auto;
        }

        > .table-container {
          flex: 1;
          overflow: auto;
          display: flex;
          flex-flow: column;

          > .current-part {
            flex: 1;
            display: flex;
            flex-flow: column;
            grid-gap: 1rem;
          }
        }
      }
    }

    > .empty-part-container {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      grid-gap: 0.5rem;
      border: 1px dashed var(--el-border-color);
      border-radius: var(--el-border-radius-base);
    }
  }
}
</style>
