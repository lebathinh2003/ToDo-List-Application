using TaskStatus = TaskService.Domain.Models.TaskStatus;

namespace TaskService.Infrastructure.Persistence.Mockup;

public static class TaskData
{
    public static readonly List<TaskMockup> Data = new List<TaskMockup>
    {
        // Bob Nguyen's tasks (5 tasks)
        new TaskMockup {
            Title = "Update website content",
            Description = "Refresh homepage banner and promotions",
            AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "BN-WEB-UPD"
        },
        new TaskMockup {
            Title = "Prepare monthly report",
            Description = "Compile sales data for April",
            AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "BN-RPT-APR"
        },
        new TaskMockup {
            Title = "Client meeting preparation",
            Description = "Prepare slides for ABC Corp presentation",
            AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.Done,
            Code = "BN-MTG-ABC"
        },
        new TaskMockup {
            Title = "Inventory check",
            Description = "Verify warehouse stock levels",
            AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "BN-INV-CHK"
        },
        new TaskMockup {
            Title = "Process customer feedback",
            Description = "Analyze and respond to recent customer surveys",
            AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.InProgress,
            Code = "BN-CUST-FDBK"
        },

        // Charlie Tran's tasks (6 tasks)
        new TaskMockup {
            Title = "Develop new feature",
            Description = "Implement user profile editing functionality",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "CT-DEV-FEAT"
        },
        new TaskMockup {
            Title = "Bug fixing",
            Description = "Resolve login issues on mobile devices",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "CT-BUG-FIX"
        },
        new TaskMockup {
            Title = "Code review",
            Description = "Review pull request #1245 from junior developer",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "CT-CODE-REV-PR1245" // Chi tiết hơn để tránh trùng lặp với các "Code review" khác
        },
        new TaskMockup {
            Title = "Database optimization",
            Description = "Improve query performance for reporting module",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "CT-DB-OPT-RPT"
        },
        new TaskMockup {
            Title = "Team training",
            Description = "Prepare materials for React workshop",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "CT-TRAIN-REACT"
        },
        new TaskMockup {
            Title = "Document API endpoints",
            Description = "Create Swagger documentation for new endpoints",
            AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "CT-API-DOC-SWAG"
        },

        // Diana Le's tasks (5 tasks)
        new TaskMockup {
            Title = "Customer support tickets",
            Description = "Handle priority 1 support cases",
            AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.Done,
            Code = "DL-SUP-TKT-P1"
        },
        new TaskMockup {
            Title = "Product training",
            Description = "Train new hires on product features",
            AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "DL-PROD-TRN"
        },
        new TaskMockup {
            Title = "Knowledge base update",
            Description = "Add new FAQ entries based on recent issues",
            AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.ToDo,
            Code = "DL-KB-UPD-FAQ"
        },
        new TaskMockup {
            Title = "Customer satisfaction survey",
            Description = "Design and distribute quarterly survey",
            AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "DL-CUST-SURVEY-Q"
        },
        new TaskMockup {
            Title = "Support workflow improvement",
            Description = "Propose improvements to ticket routing system",
            AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "DL-SUP-WF-IMPRV"
        },

        // Ethan Pham's tasks (7 tasks)
        new TaskMockup {
            Title = "Social media campaign",
            Description = "Plan and schedule May promotions",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "EP-SMM-CAMP-MAY"
        },
        new TaskMockup {
            Title = "Email newsletter",
            Description = "Create content for monthly newsletter",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "EP-EMAIL-NEWS"
        },
        new TaskMockup {
            Title = "SEO optimization",
            Description = "Improve meta tags for top 20 pages",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "EP-SEO-META"
        },
        new TaskMockup {
            Title = "Influencer collaboration",
            Description = "Identify and contact potential partners",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "EP-INFL-PARTNER"
        },
        new TaskMockup {
            Title = "Analytics report",
            Description = "Analyze April campaign performance",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "EP-ANALYTICS-APR"
        },
        new TaskMockup {
            Title = "Landing page redesign",
            Description = "Create wireframes for new product page",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "EP-LP-REDESIGN"
        },
        new TaskMockup {
            Title = "Competitor analysis",
            Description = "Research competitors' Q2 marketing strategies",
            AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "EP-COMP-ANLS-Q2"
        },

        // Fiona Do's tasks (6 tasks)
        new TaskMockup {
            Title = "Budget planning",
            Description = "Prepare Q3 department budget proposal",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(12),
            Status = TaskStatus.InProgress,
            Code = "FD-BUDGET-Q3"
        },
        new TaskMockup {
            Title = "Vendor negotiations",
            Description = "Renew contract with office supplies vendor",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "FD-VEND-RENEW"
        },
        new TaskMockup {
            Title = "Expense report",
            Description = "Process April team expenses",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "FD-EXP-RPT-APR"
        },
        new TaskMockup {
            Title = "Team building event",
            Description = "Organize quarterly team outing",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(21),
            Status = TaskStatus.ToDo,
            Code = "FD-TEAM-OUTING"
        },
        new TaskMockup {
            Title = "Software license audit",
            Description = "Verify all software licenses are up to date",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "FD-SW-AUDIT"
        },
        new TaskMockup {
            Title = "New hire onboarding",
            Description = "Prepare materials for new accountant starting next week",
            AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.ToDo,
            Code = "FD-ONBOARD-ACC"
        },

        // George Vo's tasks (5 tasks)
        new TaskMockup {
            Title = "Server maintenance",
            Description = "Perform monthly server updates and patches",
            AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.Done,
            Code = "GV-SRV-MAINT"
        },
        new TaskMockup {
            Title = "Backup verification",
            Description = "Test disaster recovery procedures",
            AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "GV-BCKUP-DR"
        },
        new TaskMockup {
            Title = "Network security audit",
            Description = "Check firewall rules and VPN access",
            AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "GV-NET-SEC-AUDIT"
        },
        new TaskMockup {
            Title = "New office setup",
            Description = "Configure IT infrastructure for new branch office",
            AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "GV-OFFICE-IT-SETUP"
        },
        new TaskMockup {
            Title = "Employee training",
            Description = "Conduct cybersecurity awareness session",
            AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "GV-EMP-TRAIN-CYBER"
        },

        // Hannah Nguyen's tasks (6 tasks)
        new TaskMockup {
            Title = "UI redesign",
            Description = "Create mockups for dashboard redesign",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "HN-UI-DASH-REDESIGN"
        },
        new TaskMockup {
            Title = "User testing",
            Description = "Organize usability testing sessions",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "HN-USER-TEST-SESS"
        },
        new TaskMockup {
            Title = "Design system update",
            Description = "Add new components to design library",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "HN-DESIGN-SYS-COMP"
        },
        new TaskMockup {
            Title = "Mobile app icons",
            Description = "Create new icon set for mobile application",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "HN-MOBILE-ICONS"
        },
        new TaskMockup {
            Title = "Brand guidelines",
            Description = "Update visual identity documentation",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.InProgress,
            Code = "HN-BRAND-GUIDE-UPD"
        },
        new TaskMockup {
            Title = "Accessibility audit",
            Description = "Check color contrast and screen reader compatibility",
            AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "HN-A11Y-AUDIT"
        },

        // Ian Tran's tasks (5 tasks)
        new TaskMockup {
            Title = "Database migration",
            Description = "Plan migration from MySQL to PostgreSQL",
            AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "IT-DB-MIG-PGSQL"
        },
        new TaskMockup {
            Title = "Performance tuning",
            Description = "Optimize slow-running queries",
            AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "IT-PERF-TUNE-SQL"
        },
        new TaskMockup {
            Title = "Backup automation",
            Description = "Implement automated backup verification",
            AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "IT-BCKUP-AUTO-VER"
        },
        new TaskMockup {
            Title = "Data warehouse design",
            Description = "Create schema for new analytics warehouse",
            AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
            DueDate = DateTime.Now.AddDays(21),
            Status = TaskStatus.ToDo,
            Code = "IT-DWH-SCHEMA"
        },
        new TaskMockup {
            Title = "Security patches",
            Description = "Apply latest database security updates",
            AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "IT-SEC-PATCH-DB"
        },

        // Jade Pham's tasks (7 tasks)
        new TaskMockup {
            Title = "Content calendar",
            Description = "Plan blog topics for next quarter",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "JP-CONT-CAL-Q"
        },
        new TaskMockup {
            Title = "Case study",
            Description = "Write customer success story for Client X",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "JP-CASE-STUDY-CLX"
        },
        new TaskMockup {
            Title = "SEO content",
            Description = "Create 10 product pages optimized for search",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "JP-SEO-CONT-PROD"
        },
        new TaskMockup {
            Title = "Social media posts",
            Description = "Draft 20 posts for May schedule",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "JP-SMM-POSTS-MAY"
        },
        new TaskMockup {
            Title = "Email campaign",
            Description = "Write copy for product launch email sequence",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "JP-EMAIL-CAMP-LAUNCH"
        },
        new TaskMockup {
            Title = "Video script",
            Description = "Write tutorial video script for new feature",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.ToDo,
            Code = "JP-VID-SCRIPT-FEAT"
        },
        new TaskMockup {
            Title = "Press release",
            Description = "Draft announcement for partnership with Company Y",
            AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "JP-PRESS-REL-COMY"
        },

        // Kevin Do's tasks (5 tasks)
        new TaskMockup {
            Title = "Sales pipeline review",
            Description = "Analyze Q2 sales opportunities",
            AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "KD-SALES-PIPE-Q2"
        },
        new TaskMockup {
            Title = "Client proposal",
            Description = "Prepare custom solution for Enterprise Client Z",
            AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "KD-CLIENT-PROP-ENTZ"
        },
        new TaskMockup {
            Title = "Sales training",
            Description = "Train new reps on product positioning",
            AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "KD-SALES-TRN-REP"
        },
        new TaskMockup {
            Title = "Competitive analysis",
            Description = "Update battle cards with latest competitor info",
            AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "KD-COMP-ANLS-BATTLE"
        },
        new TaskMockup {
            Title = "Quarterly targets",
            Description = "Set individual sales targets for Q3",
            AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "KD-QTR-TARGETS-Q3"
        },

        // Lily Le's tasks (6 tasks)
        new TaskMockup {
            Title = "Recruitment campaign",
            Description = "Plan job ads for developer positions",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "LL-RECRUIT-DEV"
        },
        new TaskMockup {
            Title = "Interview scheduling",
            Description = "Coordinate technical interviews for candidates",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.ToDo,
            Code = "LL-INT-SCHED-TECH"
        },
        new TaskMockup {
            Title = "Onboarding program",
            Description = "Revise new hire orientation materials",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "LL-ONBOARD-REV"
        },
        new TaskMockup {
            Title = "Employee survey",
            Description = "Analyze results from engagement survey",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.Done,
            Code = "LL-EMP-SURVEY-ANLS"
        },
        new TaskMockup {
            Title = "Performance reviews",
            Description = "Schedule mid-year review meetings",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "LL-PERF-REV-MIDYR"
        },
        new TaskMockup {
            Title = "Training needs assessment",
            Description = "Identify skill gaps in engineering team",
            AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "LL-TRAIN-ASSESS-ENG"
        },

        // Mike Nguyen's tasks (5 tasks)
        new TaskMockup {
            Title = "Product roadmap",
            Description = "Update 12-month product development plan",
            AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "MN-PROD-ROADMAP-YR"
        },
        new TaskMockup {
            Title = "Feature prioritization",
            Description = "Score and rank backlog items",
            AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "MN-FEAT-PRIORITIZE"
        },
        new TaskMockup {
            Title = "Stakeholder meeting",
            Description = "Prepare presentation for executive review",
            AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "MN-STAKEHOLD-MTG-EXEC"
        },
        new TaskMockup {
            Title = "Competitor analysis",
            Description = "Research competitor product updates",
            AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "MN-COMP-ANLS-PROD"
        },
        new TaskMockup {
            Title = "User feedback synthesis",
            Description = "Analyze recent user testing results",
            AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.InProgress,
            Code = "MN-USER-FDBK-SYNTH"
        },

        // Nina Vo's tasks (6 tasks)
        new TaskMockup {
            Title = "Legal document review",
            Description = "Review new vendor contracts",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "NV-LEGAL-DOC-VEND"
        },
        new TaskMockup {
            Title = "Compliance audit",
            Description = "Prepare documentation for GDPR compliance check",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "NV-COMPL-AUDIT-GDPR"
        },
        new TaskMockup {
            Title = "Policy update",
            Description = "Revise remote work policy",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "NV-POLICY-UPD-REMOTE"
        },
        new TaskMockup {
            Title = "Trademark filing",
            Description = "Submit paperwork for new product trademark",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "NV-TRADEMARK-PROD"
        },
        new TaskMockup {
            Title = "Employee agreement",
            Description = "Update standard employment contract",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.InProgress,
            Code = "NV-EMP-AGREE-STD"
        },
        new TaskMockup {
            Title = "Risk assessment",
            Description = "Evaluate risks for new market expansion",
            AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "NV-RISK-ASSESS-MKT"
        },

        // Oscar Bui's tasks (5 tasks)
        new TaskMockup {
            Title = "Test automation",
            Description = "Implement automated regression tests",
            AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "OB-TEST-AUTO-REG"
        },
        new TaskMockup {
            Title = "Performance testing",
            Description = "Run load tests on new API endpoints",
            AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "OB-PERF-TEST-API"
        },
        new TaskMockup {
            Title = "Test case review",
            Description = "Verify test coverage for critical features",
            AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "OB-TEST-CASE-REV"
        },
        new TaskMockup {
            Title = "Bug triage",
            Description = "Prioritize and assign recent bug reports",
            AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "OB-BUG-TRIAGE"
        },
        new TaskMockup {
            Title = "CI/CD pipeline",
            Description = "Improve test integration in deployment pipeline",
            AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "OB-CICD-PIPE-TEST"
        },

        // Paula Dang's tasks (6 tasks)
        new TaskMockup {
            Title = "Financial report",
            Description = "Prepare Q2 financial statements",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "PD-FIN-RPT-Q2"
        },
        new TaskMockup {
            Title = "Budget variance analysis",
            Description = "Analyze April budget vs actuals",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "PD-BUDGET-VAR-APR"
        },
        new TaskMockup {
            Title = "Tax preparation",
            Description = "Gather documents for quarterly tax filing",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "PD-TAX-PREP-Q"
        },
        new TaskMockup {
            Title = "Audit preparation",
            Description = "Prepare for annual financial audit",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "PD-AUDIT-PREP-ANN"
        },
        new TaskMockup {
            Title = "Expense policy update",
            Description = "Revise travel and expense policy",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "PD-EXP-POLICY-REV"
        },
        new TaskMockup {
            Title = "Forecast update",
            Description = "Update annual revenue forecast",
            AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "PD-FORECAST-UPD-ANN"
        },

        // Quincy Lam's tasks (5 tasks)
        new TaskMockup {
            Title = "Market research",
            Description = "Study potential expansion into Southeast Asia",
            AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "QL-MKT-RSRCH-SEA"
        },
        new TaskMockup {
            Title = "Customer segmentation",
            Description = "Analyze customer data for targeting strategy",
            AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "QL-CUST-SEGMENT"
        },
        new TaskMockup {
            Title = "Competitive positioning",
            Description = "Develop new positioning statement",
            AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.Done,
            Code = "QL-COMP-POSITION"
        },
        new TaskMockup {
            Title = "Pricing analysis",
            Description = "Evaluate pricing strategy vs competitors",
            AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "QL-PRICE-ANLS-COMP"
        },
        new TaskMockup {
            Title = "Partnership evaluation",
            Description = "Assess potential strategic partners",
            AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.InProgress,
            Code = "QL-PARTNER-EVAL"
        },

        // Rita Ngo's tasks (6 tasks)
        new TaskMockup {
            Title = "Office relocation",
            Description = "Coordinate move to new headquarters",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(21),
            Status = TaskStatus.ToDo,
            Code = "RN-OFFICE-RELOC-HQ"
        },
        new TaskMockup {
            Title = "Vendor management",
            Description = "Renew cleaning service contract",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "RN-VEND-MGMT-CLEAN"
        },
        new TaskMockup {
            Title = "Supply ordering",
            Description = "Order office supplies for next quarter",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "RN-SUPPLY-ORDER-Q"
        },
        new TaskMockup {
            Title = "Facility maintenance",
            Description = "Schedule HVAC system inspection",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "RN-FACILITY-HVAC"
        },
        new TaskMockup {
            Title = "Event planning",
            Description = "Organize company anniversary celebration",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(30),
            Status = TaskStatus.InProgress,
            Code = "RN-EVENT-PLAN-ANNIV"
        },
        new TaskMockup {
            Title = "Space planning",
            Description = "Design new office layout for hybrid work",
            AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "RN-SPACE-PLAN-HYBRID"
        },

        // Steve Vu
        new TaskMockup {
            AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "SV-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.InProgress,
            Code = "SV-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "SV-CODE-REV"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
            Title = "Update documentation",
            Description = "Update API documentation with new endpoints.",
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "SV-DOC-UPD-API"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
            Title = "Test payment gateway",
            Description = "Test and validate the payment integration.",
            DueDate = DateTime.Now.AddDays(15),
            Status = TaskStatus.Cancelled,
            Code = "SV-TEST-PAY-GW"
        },

        // Tina Ho
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Design new feature",
            Description = "Work on the UI/UX for the upcoming feature.",
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.InProgress,
            Code = "TH-DESIGN-FEAT-UIUX"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "TH-TEAM-MTG-WKLY"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Optimize database queries",
            Description = "Improve performance of database operations.",
            DueDate = DateTime.Now.AddDays(20),
            Status = TaskStatus.ToDo,
            Code = "TH-DB-OPT-PERF"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.Done,
            Code = "TH-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(6),
            Status = TaskStatus.InProgress,
            Code = "TH-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
            Title = "Update documentation",
            Description = "Update API documentation with new endpoints.",
            DueDate = DateTime.Now.AddDays(14),
            Status = TaskStatus.ToDo,
            Code = "TH-DOC-UPD-API"
        },

        // Uyen Mai
        new TaskMockup {
            AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.InProgress,
            Code = "UM-CODE-REV"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
            Title = "Test payment gateway",
            Description = "Test and validate the payment integration.",
            DueDate = DateTime.Now.AddDays(12),
            Status = TaskStatus.ToDo,
            Code = "UM-TEST-PAY-GW"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.Cancelled,
            Code = "UM-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
            Title = "Design new feature",
            Description = "Work on the UI/UX for the upcoming feature.",
            DueDate = DateTime.Now.AddDays(9),
            Status = TaskStatus.ToDo,
            Code = "UM-DESIGN-FEAT-UIUX"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "UM-BUG-LOGIN"
        },

        // Victor Tran
        new TaskMockup {
            AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
            Title = "Optimize database queries",
            Description = "Improve performance of database operations.",
            DueDate = DateTime.Now.AddDays(11),
            Status = TaskStatus.InProgress,
            Code = "VT-DB-OPT-PERF"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "VT-TEAM-MTG-WKLY"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.Done,
            Code = "VT-CODE-REV"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.InProgress,
            Code = "VT-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
            Title = "Update documentation",
            Description = "Update API documentation with new endpoints.",
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.ToDo,
            Code = "VT-DOC-UPD-API"
        },

        // Wendy Dang
        new TaskMockup {
            AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
            Title = "Test payment gateway",
            Description = "Test and validate the payment integration.",
            DueDate = DateTime.Now.AddDays(6),
            Status = TaskStatus.InProgress,
            Code = "WD-TEST-PAY-GW"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "WD-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "WD-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(2),
            Status = TaskStatus.ToDo,
            Code = "WD-TEAM-MTG-WKLY"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
            Title = "Update documentation",
            Description = "Update API documentation with new endpoints.",
            DueDate = DateTime.Now.AddDays(11),
            Status = TaskStatus.InProgress,
            Code = "WD-DOC-UPD-API"
        },

        // Xuan Bui
        new TaskMockup {
            AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
            Title = "Optimize database queries",
            Description = "Improve performance of database operations.",
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.ToDo,
            Code = "XB-DB-OPT-PERF"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "XB-CODE-REV"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.ToDo,
            Code = "XB-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(12),
            Status = TaskStatus.Done,
            Code = "XB-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "XB-TEAM-MTG-WKLY"
        },

        // Yen Vo
        new TaskMockup {
            AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
            Title = "Update documentation",
            Description = "Update API documentation with new endpoints.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "YV-DOC-UPD-API"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
            Title = "Test payment gateway",
            Description = "Test and validate the payment integration.",
            DueDate = DateTime.Now.AddDays(9),
            Status = TaskStatus.InProgress,
            Code = "YV-TEST-PAY-GW"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(6),
            Status = TaskStatus.Done,
            Code = "YV-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.ToDo,
            Code = "YV-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.InProgress,
            Code = "YV-CODE-REV"
        },

        // Zack Le
        new TaskMockup {
            AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
            Title = "Design new feature",
            Description = "Work on the UI/UX for the upcoming feature.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "ZL-DESIGN-FEAT-UIUX"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
            Title = "Optimize database queries",
            Description = "Improve performance of database operations.",
            DueDate = DateTime.Now.AddDays(10),
            Status = TaskStatus.InProgress,
            Code = "ZL-DB-OPT-PERF"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(3),
            Status = TaskStatus.Done,
            Code = "ZL-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(9),
            Status = TaskStatus.ToDo,
            Code = "ZL-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "ZL-TEAM-MTG-WKLY"
        },

        // Amy Chau
        new TaskMockup {
            AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
            Title = "Test payment gateway",
            Description = "Test and validate the payment integration.",
            DueDate = DateTime.Now.AddDays(8),
            Status = TaskStatus.InProgress,
            Code = "AC-TEST-PAY-GW"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
            Title = "Code review",
            Description = "Review pull requests from the team.",
            DueDate = DateTime.Now.AddDays(5),
            Status = TaskStatus.ToDo,
            Code = "AC-CODE-REV"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
            Title = "Fix login bug",
            Description = "Investigate and resolve the login issue.",
            DueDate = DateTime.Now.AddDays(4),
            Status = TaskStatus.Done,
            Code = "AC-BUG-LOGIN"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
            Title = "Prepare monthly report",
            Description = "Complete the detailed report for the month.",
            DueDate = DateTime.Now.AddDays(7),
            Status = TaskStatus.ToDo,
            Code = "AC-RPT-MONTH"
        },
        new TaskMockup {
            AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
            Title = "Team meeting",
            Description = "Organize the weekly team sync-up.",
            DueDate = DateTime.Now.AddDays(1),
            Status = TaskStatus.ToDo,
            Code = "AC-TEAM-MTG-WKLY"
        }
    };
}

public class TaskMockup
{
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

}
