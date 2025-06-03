<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import VariableFilter from './VariableFilter.vue'
import { useGetDefinitions } from '../composables/getDefinitions'

const model = defineModel()
const emits = defineEmits(['select'])

// region 抽屉组件
const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()
const { t } = useLocale()
watch(model, (val) => {
  if (val) {
    drawerTitle.value = t('el.variable.selectVariable')
    openDrawer('1000')
  }
})
watch(showDrawer, (val) => {
  model.value = val
})
const onDrawerOpened = async () => {
  await getDefinitions()
}
// endregion

// region 筛选
const loadingFilter = ref(false)
const onFilter = async (filters) => {
  nameFilter.value = filters.name

  await getDefinitions()
}
// endregion

// region 表格数据
const {
  loading: loadingTableData,
  data: tableData,
  pageIndex,
  pageSize,
  nameFilter,
  getDefinitions,
  handlePageChange,
  handlePageSizeChange,
} = useGetDefinitions()
// endregion

// region 处理选择的变量
const onSelect = async (row) => {
  closeDrawer()
  emits('select', JSON.parse(JSON.stringify(row)))
}
// endregion
</script>

<template>
  <ElDrawer
    v-model="showDrawer"
    :title="drawerTitle"
    :size="drawerSize"
    @opened="onDrawerOpened"
    destroy-on-close
  >
    <div class="select-variable-root">
      <VariableFilter @filter="onFilter" :loading="loadingFilter">
      </VariableFilter>
      <ElTable
        class="table"
        stripe
        show-overflow-tooltip
        :data="tableData.items"
        v-loading="loadingTableData"
      >
        <ElTableColumn
          :label="t('el.variable.connectorInstance')"
          prop="connectorInstance"
          width="170"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.connectionName')"
          prop="connectionName"
          width="180"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.dataPointName')"
          prop="dataPointName"
          width="150"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.name')"
          prop="name"
          min-width="240"
        >
        </ElTableColumn>
        <ElTableColumn fixed="right" width="100">
          <template #default="{ row }">
            <ElButton type="primary" plain size="small" @click="onSelect(row)">
              {{ t('el.variable.select') }}
            </ElButton>
          </template>
        </ElTableColumn>
      </ElTable>
      <!--suppress JSValidateTypes -->
      <ElPagination
        layout="prev, pager, next, sizes, total"
        :total="tableData.total"
        :default-page-size="20"
        :pager-count="5"
        v-model:current-page="pageIndex"
        @update:current-page="handlePageChange"
        v-model:page-size="pageSize"
        :page-sizes="[20, 50, 100]"
        @update:page-size="handlePageSizeChange"
        :disabled="loadingTableData"
      >
      </ElPagination>
    </div>
  </ElDrawer>
</template>

<style scoped lang="scss">
.select-variable-root {
  height: 100%;
  display: flex;
  flex-flow: column;
  overflow: auto;

  > .table {
    flex: 1;
    margin: 1rem 0;
    border-top: 1px solid var(--el-border-color);
  }
}
</style>
