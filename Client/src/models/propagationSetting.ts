import { PropagationRank, DuplicationStrategy } from './enums';

export class PropagationSetting {
    id?: number;
    name?: string;
    propagationPriority: PropagationRank[];
    duplicationStrategy: DuplicationStrategy;
    contentTypes?: string[];
    propertyAlias?: string;
    fallbackSetting?: PropagationSetting;

    constructor(params: {
        id?: number;
        name?: string;
        propagationPriority?: PropagationRank[];
        duplicationStrategy?: DuplicationStrategy;
        contentTypes?: string[];
        propertyAlias?: string;
        fallbackSetting?: PropagationSetting;
    }) {
        this.id = params.id;
        this.name = params.name;
        this.propagationPriority = params.propagationPriority ?? [
            PropagationRank.Relevant,
            PropagationRank.Newest,
            PropagationRank.MostPopular,
        ];
        this.duplicationStrategy = params.duplicationStrategy ?? DuplicationStrategy.UniquePage;
        this.contentTypes = params.contentTypes;
        this.propertyAlias = params.propertyAlias;
        this.fallbackSetting = params.fallbackSetting;
    }
}
