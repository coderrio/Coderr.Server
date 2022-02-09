import { Component, Watch, Vue } from 'vue-property-decorator';

@Component
export default class ManageMenuComponent extends Vue {

    currentApplicationId = 1;

    @Watch('$route.params.applicationId')
    onApplicationIdChanged(value: string, oldValue: string) {
        this.currentApplicationId = parseInt(value, 10);
    }

    mounted() {
    }

}
