import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
    components: {
        TopMenu: require('./navmenu/navmenu.vue.html').default
    }
})
export default class AppComponent extends Vue {
}
