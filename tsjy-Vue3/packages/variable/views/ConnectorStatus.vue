<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import { getConnectorStatusApi } from '../apis/connectorStatus'
import { dayjs } from 'element-plus'

const { t } = useLocale()

const tableData = ref([])

const loading = ref(false)
const getConnectorStatus = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getConnectorStatusApi()
    buildTableData(res.data)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

let tableItemId = 0
const buildTableData = (data) => {
  tableItemId = 0
  tableData.value = []

  data.forEach((item) => {
    const ts = item['ts']
    let connectorItem = {
      id: tableItemId++,
      name: item['connectorInstance'],
      status: item['connectorStatus'],
      type: 'connector',
      ts: ts,
      children: [],
    }

    if (item['connectionStatus']) {
      item['connectionStatus'].forEach((item) => {
        let connectionItem = {
          id: tableItemId++,
          name: item['name'],
          status: item['status'],
          ts: ts,
          type: 'connection',
        }
        connectorItem.children.push(connectionItem)
      })
    }
    tableData.value.push(connectorItem)
  })
}

onMounted(async () => {
  await getConnectorStatus()
})
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.variable.connectorStatus.connectorStatus') }}
    </template>
    <template #default>
      <ElTable
        :data="tableData"
        stripe
        show-overflow-tooltip
        row-key="id"
        default-expand-all
      >
        <ElTableColumn
          :label="t('el.variable.connectorStatus.name')"
          prop="name"
          min-width="180"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.connectorStatus.type')"
          prop="type"
          min-width="180"
        >
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.connectorStatus.status')"
          prop="status"
          min-width="100"
        >
          <template #default="{ row }">
            <ElTag
              v-if="row['status'] === 'good' || row['status'] === 'available'"
              type="success"
            >
              {{ row['status'] }}
            </ElTag>
            <ElTag
              v-else-if="
                row['status'] === 'bad' || row['status'] === 'unavailable'
              "
              type="danger"
            >
              {{ row['status'] }}
            </ElTag>
            <ElTag v-else>unknown</ElTag>
          </template>
        </ElTableColumn>
        <ElTableColumn
          :label="t('el.variable.connectorStatus.lastUpdated')"
          prop="ts"
          min-width="180"
        >
          <template #default="{ row }">
            {{ dayjs(row['ts']).format('YYYY-MM-DD HH:mm:ss') }}
          </template>
        </ElTableColumn>
      </ElTable>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss"></style>
