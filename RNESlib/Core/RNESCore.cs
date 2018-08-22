using RNESlib.Core;
using Unity;

namespace RNESlib
{
    public class RNESCore
    {
        private readonly KnowlegdeBase _knowlegdeBase;
        private readonly InferenceEngine _inferenceEngine;

        public RNESCore()
        {
            var unityContainer = new UnityContainer();
            _knowlegdeBase = new KnowlegdeBase(unityContainer);
            _inferenceEngine = new InferenceEngine(unityContainer);
        }

        public void RegisterKnowledge<KnowledgeInterface, KnowledgeClass>() where KnowledgeClass : KnowledgeInterface
        {
            _knowlegdeBase.RegisterComponent<KnowledgeInterface, KnowledgeClass>();
        }

        public void RegisterSkill<SkillInterface, SkillClass>() where SkillClass : SkillInterface
        {
            _inferenceEngine.RegisterComponent<SkillInterface, SkillClass>();
        }

        public KnowledgeInterface ResolveKnowledge<KnowledgeInterface>()
        {
            return _knowlegdeBase.ResolveComponent<KnowledgeInterface>();
        }

        public SkillInterface ResolveSkill<SkillInterface>()
        {
            return _inferenceEngine.ResolveComponent<SkillInterface>();
        }

        public void RegisterKnowledgeInstance<KnowledgeInterface>(KnowledgeInterface knowledge) 
        {
            _knowlegdeBase.RegisterComponentinstance<KnowledgeInterface>(knowledge);
        }
    }
}
