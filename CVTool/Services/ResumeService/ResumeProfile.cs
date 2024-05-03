using AutoMapper;
using CVTool.Data.Model;
using CVTool.Models.AddResume;
using CVTool.Models.Common;
using CVTool.Models.EditResume;
using CVTool.Models.GetResume;

namespace CVTool.Services.ResumeService
{
    public class ResumeProfile: Profile
    {
        public ResumeProfile()
        {
            CreateMap<AddResumeRequestDTO, Resume>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components.Select(
                    c => new Component
                    {
                        ComponentDocumentId = c.ComponentDocumentId,
                        ComponentType = c.ComponentType,
                        ComponentEntries = c.ComponentEntries.Select(ce => new ComponentEntry
                        {
                            Label = ce.Label,
                            Value = ce.Value,
                            Children = ce.Children.Select(cc =>
                            new ComponentChildEntry
                            {
                                Label = cc.Label,
                                Value = cc.Value
                            }).ToList()
                        }).ToList()
                    }).ToList()));

            CreateMap<AddResumeRequestDTO, Resume>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components.Select(
                    c => new Component
                    {
                        ComponentDocumentId = c.ComponentDocumentId,
                        ComponentType = c.ComponentType,
                        ComponentEntries = c.ComponentEntries.Select(ce => new ComponentEntry
                        {
                            Label = ce.Label,
                            Value = ce.Value,
                            Children = ce.Children.Select(cc =>
                            new ComponentChildEntry
                            {
                                Label = cc.Label,
                                Value = cc.Value
                            }).ToList()
                        }).ToList()
                    }).ToList()));

            CreateMap<EditResumeRequestDTO, Resume>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components.Select(
                    c => new Component
                    {
                        ComponentDocumentId = c.ComponentDocumentId,
                        ComponentType = c.ComponentType,
                        ComponentEntries = c.ComponentEntries.Select(ce => new ComponentEntry
                        {
                            Label = ce.Label,
                            Value = ce.Value,
                            Children = ce.Children.Select(cc =>
                            new ComponentChildEntry
                            {
                                Label = cc.Label,
                                Value = cc.Value
                            }).ToList()
                        }).ToList()
                    }).ToList()));

            CreateMap<Resume, GetResumeResponseDTO > ()
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components.Select(
                    c => new ComponentDTO
                    {
                        ComponentDocumentId = c.ComponentDocumentId,
                        ComponentType = c.ComponentType,
                        ComponentEntries = c.ComponentEntries.Select(ce => new ComponentEntryDTO
                        {
                            Label = ce.Label,
                            Value = ce.Value,
                            Children = ce.Children.Select(cc =>
                            new ComponentChildEntryDTO
                            {
                                Label = cc.Label,
                                Value = cc.Value
                            }).ToList()
                        }).ToList()
                    }).ToList()));
        }
    }
}
