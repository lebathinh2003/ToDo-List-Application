using TaskStatus = TaskService.Domain.Models.TaskStatus;

namespace TaskService.Infrastructure.Persistence.Mockup;

public static class TaskData
{
    public static readonly List<TaskMockup> Data = [
        // Bob Nguyen's tasks (5 tasks)
        new TaskMockup {
        Title = "Update website content",
        Description = "Refresh homepage banner and promotions",
        AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Prepare monthly report",
        Description = "Compile sales data for April",
        AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Client meeting preparation",
        Description = "Prepare slides for ABC Corp presentation",
        AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Inventory check",
        Description = "Verify warehouse stock levels",
        AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Process customer feedback",
        Description = "Analyze and respond to recent customer surveys",
        AssigneeId = Guid.Parse("04b9f052-a44f-4b03-8f1d-4142c987a0a8"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.InProgress
    },

    // Charlie Tran's tasks (6 tasks)
    new TaskMockup {
        Title = "Develop new feature",
        Description = "Implement user profile editing functionality",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Bug fixing",
        Description = "Resolve login issues on mobile devices",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Code review",
        Description = "Review pull request #1245 from junior developer",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Database optimization",
        Description = "Improve query performance for reporting module",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Team training",
        Description = "Prepare materials for React workshop",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Document API endpoints",
        Description = "Create Swagger documentation for new endpoints",
        AssigneeId = Guid.Parse("597207f8-11e8-4fd2-b782-cacda164a507"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },

    // Diana Le's tasks (5 tasks)
    new TaskMockup {
        Title = "Customer support tickets",
        Description = "Handle priority 1 support cases",
        AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Product training",
        Description = "Train new hires on product features",
        AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Knowledge base update",
        Description = "Add new FAQ entries based on recent issues",
        AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Customer satisfaction survey",
        Description = "Design and distribute quarterly survey",
        AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Support workflow improvement",
        Description = "Propose improvements to ticket routing system",
        AssigneeId = Guid.Parse("129d006a-45c3-4151-897e-b594cf95890f"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },

    // Ethan Pham's tasks (7 tasks)
    new TaskMockup {
        Title = "Social media campaign",
        Description = "Plan and schedule May promotions",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Email newsletter",
        Description = "Create content for monthly newsletter",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "SEO optimization",
        Description = "Improve meta tags for top 20 pages",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Influencer collaboration",
        Description = "Identify and contact potential partners",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Analytics report",
        Description = "Analyze April campaign performance",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Landing page redesign",
        Description = "Create wireframes for new product page",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Competitor analysis",
        Description = "Research competitors' Q2 marketing strategies",
        AssigneeId = Guid.Parse("87e87e91-2761-43d2-9e5e-1f2bc594ef56"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },

    // Fiona Do's tasks (6 tasks)
    new TaskMockup {
        Title = "Budget planning",
        Description = "Prepare Q3 department budget proposal",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(12),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Vendor negotiations",
        Description = "Renew contract with office supplies vendor",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Expense report",
        Description = "Process April team expenses",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Team building event",
        Description = "Organize quarterly team outing",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(21),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Software license audit",
        Description = "Verify all software licenses are up to date",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "New hire onboarding",
        Description = "Prepare materials for new accountant starting next week",
        AssigneeId = Guid.Parse("2938ca79-d6f8-4657-acf6-ac562e2931ac"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.ToDo
    },

    // George Vo's tasks (5 tasks)
    new TaskMockup {
        Title = "Server maintenance",
        Description = "Perform monthly server updates and patches",
        AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Backup verification",
        Description = "Test disaster recovery procedures",
        AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Network security audit",
        Description = "Check firewall rules and VPN access",
        AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "New office setup",
        Description = "Configure IT infrastructure for new branch office",
        AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Employee training",
        Description = "Conduct cybersecurity awareness session",
        AssigneeId = Guid.Parse("76ca41c7-2451-4724-8db5-92d6ae4dc4be"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },

    // Hannah Nguyen's tasks (6 tasks)
    new TaskMockup {
        Title = "UI redesign",
        Description = "Create mockups for dashboard redesign",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "User testing",
        Description = "Organize usability testing sessions",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Design system update",
        Description = "Add new components to design library",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Mobile app icons",
        Description = "Create new icon set for mobile application",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Brand guidelines",
        Description = "Update visual identity documentation",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Accessibility audit",
        Description = "Check color contrast and screen reader compatibility",
        AssigneeId = Guid.Parse("a35f6e52-6f5d-4c39-8c60-8a3a504e261f"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },

    // Ian Tran's tasks (5 tasks)
    new TaskMockup {
        Title = "Database migration",
        Description = "Plan migration from MySQL to PostgreSQL",
        AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Performance tuning",
        Description = "Optimize slow-running queries",
        AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Backup automation",
        Description = "Implement automated backup verification",
        AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Data warehouse design",
        Description = "Create schema for new analytics warehouse",
        AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
        DueDate = DateTime.Now.AddDays(21),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Security patches",
        Description = "Apply latest database security updates",
        AssigneeId = Guid.Parse("b63fcbf4-f29a-43e2-b109-e99e9342b39c"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },

    // Jade Pham's tasks (7 tasks)
    new TaskMockup {
        Title = "Content calendar",
        Description = "Plan blog topics for next quarter",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Case study",
        Description = "Write customer success story for Client X",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "SEO content",
        Description = "Create 10 product pages optimized for search",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Social media posts",
        Description = "Draft 20 posts for May schedule",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Email campaign",
        Description = "Write copy for product launch email sequence",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Video script",
        Description = "Write tutorial video script for new feature",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Press release",
        Description = "Draft announcement for partnership with Company Y",
        AssigneeId = Guid.Parse("182dc969-59ec-4fe1-9d17-b7e8e0d32b0f"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },

    // Kevin Do's tasks (5 tasks)
    new TaskMockup {
        Title = "Sales pipeline review",
        Description = "Analyze Q2 sales opportunities",
        AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Client proposal",
        Description = "Prepare custom solution for Enterprise Client Z",
        AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Sales training",
        Description = "Train new reps on product positioning",
        AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Competitive analysis",
        Description = "Update battle cards with latest competitor info",
        AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Quarterly targets",
        Description = "Set individual sales targets for Q3",
        AssigneeId = Guid.Parse("c5772a5d-f5e6-4e9f-925c-14c36da81669"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },

    // Lily Le's tasks (6 tasks)
    new TaskMockup {
        Title = "Recruitment campaign",
        Description = "Plan job ads for developer positions",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Interview scheduling",
        Description = "Coordinate technical interviews for candidates",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Onboarding program",
        Description = "Revise new hire orientation materials",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Employee survey",
        Description = "Analyze results from engagement survey",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Performance reviews",
        Description = "Schedule mid-year review meetings",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Training needs assessment",
        Description = "Identify skill gaps in engineering team",
        AssigneeId = Guid.Parse("f40c7c89-8085-413f-977f-b1842de1c01d"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },

    // Mike Nguyen's tasks (5 tasks)
    new TaskMockup {
        Title = "Product roadmap",
        Description = "Update 12-month product development plan",
        AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Feature prioritization",
        Description = "Score and rank backlog items",
        AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Stakeholder meeting",
        Description = "Prepare presentation for executive review",
        AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Competitor analysis",
        Description = "Research competitor product updates",
        AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "User feedback synthesis",
        Description = "Analyze recent user testing results",
        AssigneeId = Guid.Parse("7a38dc3d-6d8b-4c7d-b3c2-e9913fe0ecb6"),
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.InProgress
    },

    // Nina Vo's tasks (6 tasks)
    new TaskMockup {
        Title = "Legal document review",
        Description = "Review new vendor contracts",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Compliance audit",
        Description = "Prepare documentation for GDPR compliance check",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Policy update",
        Description = "Revise remote work policy",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Trademark filing",
        Description = "Submit paperwork for new product trademark",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Employee agreement",
        Description = "Update standard employment contract",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Risk assessment",
        Description = "Evaluate risks for new market expansion",
        AssigneeId = Guid.Parse("b5fc929a-cf5a-4d17-b706-c3a03a1f9444"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },

    // Oscar Bui's tasks (5 tasks)
    new TaskMockup {
        Title = "Test automation",
        Description = "Implement automated regression tests",
        AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Performance testing",
        Description = "Run load tests on new API endpoints",
        AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Test case review",
        Description = "Verify test coverage for critical features",
        AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Bug triage",
        Description = "Prioritize and assign recent bug reports",
        AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "CI/CD pipeline",
        Description = "Improve test integration in deployment pipeline",
        AssigneeId = Guid.Parse("42bc3853-06d9-4c34-8de4-3ef5ff488e2d"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },

    // Paula Dang's tasks (6 tasks)
    new TaskMockup {
        Title = "Financial report",
        Description = "Prepare Q2 financial statements",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Budget variance analysis",
        Description = "Analyze April budget vs actuals",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Tax preparation",
        Description = "Gather documents for quarterly tax filing",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Audit preparation",
        Description = "Prepare for annual financial audit",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Expense policy update",
        Description = "Revise travel and expense policy",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Forecast update",
        Description = "Update annual revenue forecast",
        AssigneeId = Guid.Parse("0b67c97c-6b71-4f7b-bc91-0896cb1f6e76"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },

    // Quincy Lam's tasks (5 tasks)
    new TaskMockup {
        Title = "Market research",
        Description = "Study potential expansion into Southeast Asia",
        AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Customer segmentation",
        Description = "Analyze customer data for targeting strategy",
        AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Competitive positioning",
        Description = "Develop new positioning statement",
        AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Pricing analysis",
        Description = "Evaluate pricing strategy vs competitors",
        AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Partnership evaluation",
        Description = "Assess potential strategic partners",
        AssigneeId = Guid.Parse("2e78e1e9-e3a4-498e-960a-1a7ad0f7db9b"),
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.InProgress
    },

    // Rita Ngo's tasks (6 tasks)
    new TaskMockup {
        Title = "Office relocation",
        Description = "Coordinate move to new headquarters",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(21),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Vendor management",
        Description = "Renew cleaning service contract",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Supply ordering",
        Description = "Order office supplies for next quarter",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        Title = "Facility maintenance",
        Description = "Schedule HVAC system inspection",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        Title = "Event planning",
        Description = "Organize company anniversary celebration",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(30),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        Title = "Space planning",
        Description = "Design new office layout for hybrid work",
        AssigneeId = Guid.Parse("9f1b7b3f-66aa-4c52-86bb-67fd21c7cddb"),
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },
 // Steve Vu
    new TaskMockup {
        AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
        Title = "Update documentation",
        Description = "Update API documentation with new endpoints.",
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("d792f01d-7e90-4f3b-b317-8e3c5edcb9aa"),
        Title = "Test payment gateway",
        Description = "Test and validate the payment integration.",
        DueDate = DateTime.Now.AddDays(15),
        Status = TaskStatus.Cancelled
    },

    // Tina Ho
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Design new feature",
        Description = "Work on the UI/UX for the upcoming feature.",
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Optimize database queries",
        Description = "Improve performance of database operations.",
        DueDate = DateTime.Now.AddDays(20),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(6),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("cefc6a59-d964-42f5-8120-d061574c7ba6"),
        Title = "Update documentation",
        Description = "Update API documentation with new endpoints.",
        DueDate = DateTime.Now.AddDays(14),
        Status = TaskStatus.ToDo
    },

    // Uyen Mai
    new TaskMockup {
        AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
        Title = "Test payment gateway",
        Description = "Test and validate the payment integration.",
        DueDate = DateTime.Now.AddDays(12),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.Cancelled
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
        Title = "Design new feature",
        Description = "Work on the UI/UX for the upcoming feature.",
        DueDate = DateTime.Now.AddDays(9),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("f18a1a96-7392-4e26-b1e0-4d92873bc92f"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },

    // Victor Tran
    new TaskMockup {
        AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
        Title = "Optimize database queries",
        Description = "Improve performance of database operations.",
        DueDate = DateTime.Now.AddDays(11),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("032b70a3-0eb6-4ac6-9ab6-35231e25b92e"),
        Title = "Update documentation",
        Description = "Update API documentation with new endpoints.",
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.ToDo
    },

    // Wendy Dang
    new TaskMockup {
        AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
        Title = "Test payment gateway",
        Description = "Test and validate the payment integration.",
        DueDate = DateTime.Now.AddDays(6),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(2),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("89a9a3c4-c46e-4c9f-a4f6-d4c8aef0cc13"),
        Title = "Update documentation",
        Description = "Update API documentation with new endpoints.",
        DueDate = DateTime.Now.AddDays(11),
        Status = TaskStatus.InProgress
        },

    // Xuan Bui
    new TaskMockup {
        AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
        Title = "Optimize database queries",
        Description = "Improve performance of database operations.",
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(12),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("490d364f-bd0e-4e2c-846b-f46514cd0fa4"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    },

    // Yen Vo
    new TaskMockup {
        AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
        Title = "Update documentation",
        Description = "Update API documentation with new endpoints.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
        Title = "Test payment gateway",
        Description = "Test and validate the payment integration.",
        DueDate = DateTime.Now.AddDays(9),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(6),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("01d4a313-3c71-40a6-a86f-c4cb1ac8b5fb"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.InProgress
    },

    // Zack Le
    new TaskMockup {
        AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
        Title = "Design new feature",
        Description = "Work on the UI/UX for the upcoming feature.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
        Title = "Optimize database queries",
        Description = "Improve performance of database operations.",
        DueDate = DateTime.Now.AddDays(10),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(3),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(9),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("238f2c9f-26a3-4a6e-9d65-2c06a6556c86"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    },

    // Amy Chau
    new TaskMockup {
        AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
        Title = "Test payment gateway",
        Description = "Test and validate the payment integration.",
        DueDate = DateTime.Now.AddDays(8),
        Status = TaskStatus.InProgress
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
        Title = "Code review",
        Description = "Review pull requests from the team.",
        DueDate = DateTime.Now.AddDays(5),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
        Title = "Fix login bug",
        Description = "Investigate and resolve the login issue.",
        DueDate = DateTime.Now.AddDays(4),
        Status = TaskStatus.Done
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
        Title = "Prepare monthly report",
        Description = "Complete the detailed report for the month.",
        DueDate = DateTime.Now.AddDays(7),
        Status = TaskStatus.ToDo
    },
    new TaskMockup {
        AssigneeId = Guid.Parse("6d0c11b2-276b-46e1-8327-d3f59e12754e"),
        Title = "Team meeting",
        Description = "Organize the weekly team sync-up.",
        DueDate = DateTime.Now.AddDays(1),
        Status = TaskStatus.ToDo
    }
    ];
}

public class TaskMockup
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

}
