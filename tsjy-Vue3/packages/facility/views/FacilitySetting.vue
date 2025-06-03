<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import { getFacilitiesApi } from '../apis/facility'
import AddFacility from '../components/AddFacility.vue'
import UpdateFacility from '../components/UpdateFacility.vue'
import DeleteFacility from '../components/DeleteFacility.vue'

// region 组件
const { t } = useLocale()
onMounted(async () => {
  await getFacilities()
})
// endregion

// region 获取设备列表

/* 支持的设备比较少，因此设备表格不分页 */

const loadingGetFacilities = ref(false)
const tableData = ref()
const getFacilities = async () => {
  loadingGetFacilities.value = true
  try {
    const { data } = await getFacilitiesApi()
    tableData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetFacilities.value = false
  }
}
// endregion

// region 添加设备
const showAddFacility = ref(false)
const onAddFacility = () => {
  showAddFacility.value = true
}
const onAddFacilityFinished = async () => {
  await getFacilities()
}
// endregion

// region 更新设备
const showUpdateFacility = ref(false)
const currentUpdateFacility = ref()
const onUpdateFacility = (row) => {
  currentUpdateFacility.value = JSON.parse(JSON.stringify(row))
  showUpdateFacility.value = true
}
// endregion

// region 删除设备
const showDeleteFacility = ref(false)
const currentDeleteFacility = ref()
const onDeleteFacility = (row) => {
  currentDeleteFacility.value = JSON.parse(JSON.stringify(row))
  showDeleteFacility.value = true
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facility.setting.facilitySetting') }}
    </template>
    <template #default>
      <div class="content-container" v-loading="loadingGetFacilities">
        <div class="header-container">
          <ElButton type="primary" plain @click="onAddFacility">
            {{ t('el.facility.setting.addFacility') }}
          </ElButton>
        </div>
        <ElTable class="table" :data="tableData">
          <ElTableColumn
            type="index"
            :label="t('el.facility.setting.index')"
            width="80"
          >
          </ElTableColumn>
          <ElTableColumn
            prop="name"
            :label="t('el.facility.setting.name')"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn
            prop="serialNumber"
            :label="t('el.facility.setting.serialNumber')"
            min-width="100"
          >
          </ElTableColumn>
          <ElTableColumn
            prop="isEnabled"
            :label="t('el.facility.setting.isEnabled')"
            width="100"
          >
            <template #default="{ row }">
              <ElTag size="small" v-if="row['isEnabled']">
                {{ t('el.facility.setting.yes') }}
              </ElTag>
              <ElTag size="small" v-else type="info">
                {{ t('el.facility.setting.no') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn
            prop="description"
            :label="t('el.facility.setting.description')"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn width="150">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton type="primary" plain @click="onUpdateFacility(row)">
                  {{ t('el.facility.setting.update') }}
                </ElButton>
                <ElButton type="danger" plain @click="onDeleteFacility(row)">
                  {{ t('el.facility.setting.delete') }}
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
      </div>
    </template>
  </ContentLayout>

  <AddFacility v-model="showAddFacility" @finished="onAddFacilityFinished">
  </AddFacility>
  <UpdateFacility
    v-model="showUpdateFacility"
    :facility="currentUpdateFacility"
    @finished="async () => await getFacilities()"
  >
  </UpdateFacility>
  <DeleteFacility
    v-model="showDeleteFacility"
    :facility="currentDeleteFacility"
    @finished="async () => await getFacilities()"
  >
  </DeleteFacility>
</template>

<style scoped lang="scss">
.content-container {
  flex: 1;
  display: flex;
  flex-flow: column;

  > .header-container {
    padding-bottom: 1rem;
  }

  > .table {
    flex: 1;
  }
}
</style>
